using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Auth;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public sealed class DataTransmissionClient
    {
        public delegate void DataTransmittedEventHandler(object sender, DataTransmittedEventArgs e);

        public delegate void InitialisationFailedEventHandler(object sender, InitialisationFailedEventArgs e);

        public delegate void TransmissionFailedEventHandler(object sender, TransmissionFailedEventArgs e);

        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient());

        private PublisherClient _publisher;
        private TopicName _topicName;

        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        public bool Initialised { get; private set; }

        public event InitialisationFailedEventHandler InitialisationFailed;

        public event TransmissionFailedEventHandler TransmissionFailed;

        public event DataTransmittedEventHandler DataTransmitted;

        public async Task InitAsync(
            CloudServiceCredentials cloudServiceCredentials,
            DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            if (cloudServiceCredentials == null)
                throw new ArgumentNullException(nameof(cloudServiceCredentials));
            if (dataTransmissionClientConfigSettings == null)
                throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings));
            if (string.IsNullOrEmpty(dataTransmissionClientConfigSettings.ProjectId))
                throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings.ProjectId));
            if (string.IsNullOrEmpty(dataTransmissionClientConfigSettings.TopicId))
                throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings.TopicId));

            try
            {
                var credential = GoogleCredential
                    .FromJson(JsonConvert.SerializeObject(cloudServiceCredentials))
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);

                var settings = new PublisherClient.Settings
                {
                    BatchingSettings = new BatchingSettings(
                        dataTransmissionClientConfigSettings.ElementCountThreshold,
                        dataTransmissionClientConfigSettings.RequestByteThreshold, // todo: Load tests!
                        TimeSpan.FromSeconds(dataTransmissionClientConfigSettings.DelayThreshold))
                };

                var clientCreationSettings = new PublisherClient.ClientCreationSettings(
                    null,
                    null,
                    credential.ToChannelCredentials());

                _topicName = new TopicName(
                    dataTransmissionClientConfigSettings.ProjectId,
                    dataTransmissionClientConfigSettings.TopicId);

                _publisher = await PublisherClient.CreateAsync(_topicName, clientCreationSettings, settings);

                Initialised = true;
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while initializing the data transmission client.";
                OnInitialisationFailed(new InitialisationFailedEventArgs(new Exception(errorMessage, exception)));
            }
        }

        public async Task TransmitAsync(IEnumerable<string> eventMetadataPayloadBatch)
        {
            if (eventMetadataPayloadBatch == null)
                throw new ArgumentNullException(nameof(eventMetadataPayloadBatch));
            try
            {
                var publishTasks = eventMetadataPayloadBatch
                    .Select(eventMetadataPayload => new PubsubMessage
                        {Data = ByteString.CopyFromUtf8(eventMetadataPayload)})
                    .Select(pubsubMessage => _publisher.PublishAsync(pubsubMessage)).ToList();

                foreach (var publishTask in publishTasks) await publishTask;

                OnDataTransmitted(new DataTransmittedEventArgs(publishTasks.Count));
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while transmitting the payload.";
                OnTransmissionFailed(
                    new TransmissionFailedEventArgs(new Exception(errorMessage,
                        exception))); // todo: batch-publish events to Stackdriver; make sure jobs run regularly
            }
        }

        private void OnTransmissionFailed(TransmissionFailedEventArgs e)
        {
            TransmissionFailed?.Invoke(this, e);
        }

        private void OnInitialisationFailed(InitialisationFailedEventArgs e)
        {
            InitialisationFailed?.Invoke(this, e);
        }

        private void OnDataTransmitted(DataTransmittedEventArgs e)
        {
            DataTransmitted?.Invoke(this, e);
        }
    }
}