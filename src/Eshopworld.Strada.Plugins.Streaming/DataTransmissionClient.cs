﻿using System;
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
    /// <summary>
    ///     DataTransmissionClient is a Google Cloud Pub/Sub client, providing connectivity and transmission functionality.
    /// </summary>
    public sealed class DataTransmissionClient
    {
        public delegate void InitialisationFailedEventHandler(object sender, InitialisationFailedEventArgs e);

        public delegate void TransmissionFailedEventHandler(object sender, TransmissionFailedEventArgs e);

        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient());

        private PublisherClient _publisher;
        private TopicName _topicName;

        /// <summary>
        ///     Instance is a static instance of <see cref="DataTransmissionClient" />.
        /// </summary>
        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        /// <summary>
        ///     Indicates whether or not this instance has been initialised.
        /// </summary>
        public bool Initialised { get; private set; }

        /// <summary>
        ///     InitialisationFailed is invoked if the <see cref="InitAsync(string,string,string,bool,bool,long,int)" /> method
        ///     fails.
        /// </summary>
        public event InitialisationFailedEventHandler InitialisationFailed;

        /// <summary>
        ///     TransmissionFailed is invoked if the <see cref="TransmitAsync{T}" /> method fails.
        /// </summary>
        public event TransmissionFailedEventHandler TransmissionFailed;

        /// <summary>
        ///     InitAsync instantiates Cloud Pub/Sub connectivity components.
        /// </summary>
        /// <param name="gcpServiceCredentials">The GCP Pub/Sub service credentials.</param>
        /// <param name="dataTransmissionClientConfigSettings">
        ///     The transmission client configuration settings that define how data is transmitted.
        /// </param>
        /// <exception cref="DataTransmissionClientException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public async Task InitAsync(
            CloudServiceCredentials gcpServiceCredentials,
            DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            try
            {
                if (string.IsNullOrEmpty(dataTransmissionClientConfigSettings.ProjectId))
                    throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings.ProjectId));
                if (string.IsNullOrEmpty(dataTransmissionClientConfigSettings.TopicId))
                    throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings.TopicId));
                if (gcpServiceCredentials == null) throw new ArgumentNullException(nameof(gcpServiceCredentials));

                if (!Initialised)
                {
                    var credential = GoogleCredential
                        .FromJson(JsonConvert.SerializeObject(gcpServiceCredentials))
                        .CreateScoped(PublisherServiceApiClient.DefaultScopes);

                    PublisherClient.Settings settings = null;
                    if (dataTransmissionClientConfigSettings.BatchMode)
                        settings = new PublisherClient.Settings
                        {
                            BatchingSettings = new BatchingSettings(
                                dataTransmissionClientConfigSettings.ElementCountThreshold,
                                null,
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
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while initializing the data transmission client.";
                if (dataTransmissionClientConfigSettings.SwallowExceptions)
                    OnInitialisationFailed(
                        new InitialisationFailedEventArgs(
                            new DataTransmissionClientException(errorMessage, exception)));
                else
                    throw new DataTransmissionClientException(errorMessage, exception);
            }
        }

        public async Task TransmitAsync(
            IEnumerable<string> eventMetadataPayloadBatch,
            bool swallowExceptions = true)
        {
            if (eventMetadataPayloadBatch == null)
                throw new ArgumentNullException(nameof(eventMetadataPayloadBatch));
            try
            {
                var batch = eventMetadataPayloadBatch.ToList();
                if (batch.Any())
                {
                    var publishTasks = batch
                        .Select(eventMetadataPayload => new PubsubMessage
                            {Data = ByteString.CopyFromUtf8(eventMetadataPayload)})
                        .Select(pubsubMessage => _publisher.PublishAsync(pubsubMessage)).ToList();

                    foreach (var publishTask in publishTasks) await publishTask;
                }
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while transmitting metadata.";
                if (swallowExceptions)
                    OnTransmissionFailed(
                        new TransmissionFailedEventArgs(
                            new DataTransmissionException(errorMessage, exception)));
                else
                    throw new DataTransmissionException(errorMessage, exception);
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
    }
}