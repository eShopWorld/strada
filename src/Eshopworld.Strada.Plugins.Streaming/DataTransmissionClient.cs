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
    /// <summary>
    ///     DataTransmissionClient is a Google Cloud Pub/Sub client, providing connectivity and transmission functionality.
    /// </summary>
    public sealed class DataTransmissionClient
    {
        public delegate void InitialisationFailedEventHandler(object sender, InitialisationFailedEventArgs e);

        public delegate void ShutdownFailedEventHandler(object sender, ShutdownFailedEventArgs e);

        public delegate void TransmissionFailedEventHandler(object sender, TransmissionFailedEventArgs e);

        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient()); // todo: Remove Lazy

        private PublisherClient _publisher;
        private TopicName _topicName;

        /// <summary>
        ///     Instance is a static instance of <see cref="DataTransmissionClient" />.
        /// </summary>
        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        /// <summary>
        ///     Indicates whether or not this instance has been initialised by calling
        ///     <see cref="InitAsync(string,string,string,bool,bool,long,int)" />.
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
        ///     ShutdownFailed is invoked if the <see cref="ShutDownAsync" /> method fails.
        /// </summary>
        public event ShutdownFailedEventHandler ShutdownFailed;

        /// <summary>
        ///     InitAsync instantiates Cloud Pub/Sub connectivity components.
        /// </summary>
        /// <param name="projectId">The Cloud Pub/Sub Project ID.</param>
        /// <param name="topicId">The Cloud Pub/Sub Topic ID</param>
        /// <param name="gcpServiceCredentials">The GCP Pub/Sub service credentials in JSON format.</param>
        /// <param name="swallowExceptions">
        ///     If <c>true</c>, invokes the <see cref="InitialisationFailed" /> event on error, persisting the
        ///     exception. Otherwise, the exception is thrown.
        /// </param>
        /// <param name="batchMode">Indicates whether or not to activate Pub/Sub batch mode.</param>
        /// <param name="elementCountThreshold">
        ///     The element count (in seconds) above which further processing of a batch will
        ///     occur.
        /// </param>
        /// <param name="delayThreshold">The batch lifetime (in seconds) above which further processing of a batch will occur.</param>
        /// <exception cref="DataTransmissionClientException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public async Task InitAsync(
            string projectId,
            string topicId,
            string gcpServiceCredentials,
            bool swallowExceptions = true,
            bool batchMode = false,
            long elementCountThreshold = 1000,
            int delayThreshold = 3)
        {
            try
            {
                if (string.IsNullOrEmpty(projectId)) throw new ArgumentNullException(nameof(projectId));
                if (string.IsNullOrEmpty(topicId)) throw new ArgumentNullException(nameof(topicId));
                if (gcpServiceCredentials == null) throw new ArgumentNullException(nameof(gcpServiceCredentials));

                if (!Initialised)
                {
                    var credential = GoogleCredential
                        .FromJson(gcpServiceCredentials)
                        .CreateScoped(PublisherServiceApiClient.DefaultScopes);

                    PublisherClient.Settings settings = null;
                    if (batchMode)
                        settings = new PublisherClient.Settings
                        {
                            BatchingSettings = new BatchingSettings(
                                elementCountThreshold,
                                null,
                                TimeSpan.FromSeconds(delayThreshold))
                        };

                    var clientCreationSettings = new PublisherClient.ClientCreationSettings(
                        null,
                        null,
                        credential.ToChannelCredentials());

                    _topicName = new TopicName(projectId, topicId);
                    _publisher = await PublisherClient.CreateAsync(_topicName, clientCreationSettings, settings);
                    Initialised = true;
                }
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while initializing the data transmission client.";
                if (swallowExceptions)
                    OnInitialisationFailed(
                        new InitialisationFailedEventArgs(
                            new DataTransmissionClientException(errorMessage, exception)));
                else
                    throw new DataTransmissionClientException(errorMessage, exception);
            }
        }


        /// <summary>
        ///     InitAsync instantiates Cloud Pub/Sub connectivity components.
        /// </summary>
        /// <param name="projectId">The Cloud Pub/Sub Project ID.</param>
        /// <param name="topicId">The Cloud Pub/Sub Topic ID</param>
        /// <param name="gcpServiceCredentials">The GCP Pub/Sub service credentials.</param>
        /// <param name="swallowExceptions">
        ///     If <c>true</c>, invokes the <see cref="InitialisationFailed" /> event on error, persisting the
        ///     exception. Otherwise, the exception is thrown.
        /// </param>
        /// <param name="batchMode">Indicates whether or not to activate Pub/Sub batch mode.</param>
        /// <param name="elementCountThreshold">
        ///     The element count (in seconds) above which further processing of a batch will
        ///     occur.
        /// </param>
        /// <param name="delayThreshold">The batch lifetime (in seconds) above which further processing of a batch will occur.</param>
        /// <exception cref="DataTransmissionClientException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public async Task InitAsync(
            string projectId,
            string topicId,
            GcpServiceCredentials gcpServiceCredentials,
            bool swallowExceptions = true,
            bool batchMode = false,
            long elementCountThreshold = 1000,
            int delayThreshold = 3)
        {
            try
            {
                if (string.IsNullOrEmpty(projectId)) throw new ArgumentNullException(nameof(projectId));
                if (string.IsNullOrEmpty(topicId)) throw new ArgumentNullException(nameof(topicId));
                if (gcpServiceCredentials == null) throw new ArgumentNullException(nameof(gcpServiceCredentials));

                if (!Initialised)
                {
                    var credential = GoogleCredential
                        .FromJson(JsonConvert.SerializeObject(gcpServiceCredentials))
                        .CreateScoped(PublisherServiceApiClient.DefaultScopes);

                    PublisherClient.Settings settings = null;
                    if (batchMode)
                        settings = new PublisherClient.Settings
                        {
                            BatchingSettings = new BatchingSettings(
                                elementCountThreshold,
                                null,
                                TimeSpan.FromSeconds(delayThreshold))
                        };

                    var clientCreationSettings = new PublisherClient.ClientCreationSettings(
                        null,
                        null,
                        credential.ToChannelCredentials());

                    _topicName = new TopicName(projectId, topicId);
                    _publisher = await PublisherClient.CreateAsync(_topicName, clientCreationSettings, settings);
                    Initialised = true;
                }
            }
            catch (Exception exception)
            {
                const string errorMessage = "An error occurred while initializing the data transmission client.";
                if (swallowExceptions)
                    OnInitialisationFailed(
                        new InitialisationFailedEventArgs(
                            new DataTransmissionClientException(errorMessage, exception)));
                else
                    throw new DataTransmissionClientException(errorMessage, exception);
            }
        }

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
            GcpServiceCredentials gcpServiceCredentials,
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

                    foreach (var uriSegmentMeta in dataTransmissionClientConfigSettings.UriSegmentMeta)
                        UriMetaCache.Instance.UriSegmentMeta.Add(uriSegmentMeta);
                    foreach (var allowedHttpHeader in dataTransmissionClientConfigSettings.AllowedHttpHeaders)
                        UriMetaCache.Instance.AllowedHttpHeaders.Add(allowedHttpHeader);

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

        /// <summary>
        ///     ShutDownAsync shuts down all active Cloud Pub/Sub channels.
        /// </summary>
        /// <param name="swallowExceptions">
        ///     If <c>true</c>, invokes the <see cref="InitialisationFailed" /> event on error, persisting the
        ///     exception. Otherwise, the exception is thrown.
        /// </param>
        /// <exception cref="DataTransmissionClientException"></exception>
        public async Task ShutDownAsync(bool swallowExceptions = true)
        {
            try
            {
                if (Initialised)
                {
                    await _publisher.ShutdownAsync(
                        TimeSpan.Zero); // todo: Increase shutdown timeout and implement in Startup.cs
                    Initialised = false;
                }
            }
            catch (Exception exception)
            {
                if (swallowExceptions)
                    OnShutdownFailed(new ShutdownFailedEventArgs(new DataTransmissionClientException(
                        "An error occurred while shutting the data transmission client down.", exception)));
                else
                    throw new DataTransmissionClientException(
                        "An error occurred while shutting the data transmission client down.", exception);
            }
        }

        /// <summary>
        ///     TransmitAsync persists metadata to a connected Cloud Pub/Sub instance.
        /// </summary>
        /// <param name="brandCode">The customer reference code.</param>
        /// <param name="eventName">The name of the upstream event in which the domain model was raised.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <param name="metadata">The data model to transmit to Cloud Pub/Sub.</param>
        /// <param name="queryString">The HTTP request Query string.</param>
        /// <param name="swallowExceptions">
        ///     If <c>true</c>, invokes the <see cref="TransmissionFailed" /> event on error, persisting the
        ///     exception. Otherwise, the exception is thrown.
        /// </param>
        /// <param name="userAgent">The HTTP request User Agent header value.</param>
        /// <exception cref="DataTransmissionException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task TransmitAsync<T>( // todo: Mark non-batch mode as obsolete
            string brandCode,
            string eventName,
            string correlationId,
            T metadata,
            string userAgent,
            string queryString,
            bool swallowExceptions = true) where T : class
        {
            if (string.IsNullOrEmpty(brandCode)) throw new ArgumentNullException(nameof(brandCode));
            if (string.IsNullOrEmpty(eventName)) throw new ArgumentNullException(nameof(eventName));
            if (string.IsNullOrEmpty(correlationId)) throw new ArgumentNullException(nameof(correlationId));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));
            if (string.IsNullOrEmpty(userAgent)) throw new ArgumentNullException(nameof(userAgent));

            try
            {
                var eventTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var metaDataPayload = Functions.AddTrackingMetadataToJson(
                    JsonConvert.SerializeObject(metadata),
                    brandCode,
                    eventName,
                    correlationId,
                    userAgent,
                    queryString,
                    eventTimestamp.ToString());

                var pubsubMessage = new PubsubMessage {Data = ByteString.CopyFromUtf8(metaDataPayload)};
                await _publisher.PublishAsync(pubsubMessage);
            }
            catch (Exception exception)
            {
                if (swallowExceptions)
                    OnTransmissionFailed(
                        new TransmissionFailedEventArgs(
                            new DataTransmissionException("An error occurred while transmitting metadata.",
                                brandCode, correlationId, exception)));
                else
                    throw new DataTransmissionException("An error occurred while transmitting metadata.",
                        brandCode, correlationId, exception);
            }
        }

        /// <summary>
        ///     TransmitAsync persists metadata to a connected Cloud Pub/Sub instance.
        /// </summary>
        /// <param name="brandCode">The customer reference code.</param>
        /// <param name="eventName">The name of the upstream event in which the domain model was raised.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <param name="json">The JSON-serialised data model to transmit to Cloud Pub/Sub.</param>
        /// <param name="queryString">The HTTP request Query string.</param>
        /// <param name="swallowExceptions">
        ///     If <c>true</c>, invokes the <see cref="TransmissionFailed" /> event on error, persisting the
        ///     exception. Otherwise, the exception is thrown.
        /// </param>
        /// <param name="userAgent">The HTTP request User Agent header value.</param>
        /// <exception cref="DataTransmissionException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task TransmitAsync(
            string brandCode,
            string eventName,
            string correlationId,
            string json,
            string userAgent,
            string queryString,
            bool swallowExceptions = true)
        {
            if (string.IsNullOrEmpty(brandCode)) throw new ArgumentNullException(nameof(brandCode));
            if (string.IsNullOrEmpty(eventName)) throw new ArgumentNullException(nameof(eventName));
            if (string.IsNullOrEmpty(correlationId)) throw new ArgumentNullException(nameof(correlationId));
            if (string.IsNullOrEmpty(json)) throw new ArgumentNullException(nameof(json));
            if (string.IsNullOrEmpty(userAgent)) throw new ArgumentNullException(nameof(userAgent));

            try
            {
                var eventTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();

                var metaDataPayload = Functions.AddTrackingMetadataToJson(
                    json,
                    brandCode,
                    eventName,
                    correlationId,
                    userAgent,
                    queryString,
                    eventTimestamp.ToString());

                var pubsubMessage = new PubsubMessage {Data = ByteString.CopyFromUtf8(metaDataPayload)};
                await _publisher.PublishAsync(pubsubMessage);
            }
            catch (Exception exception)
            {
                if (swallowExceptions)
                    OnTransmissionFailed(
                        new TransmissionFailedEventArgs(
                            new DataTransmissionException("An error occurred while transmitting metadata.",
                                brandCode, correlationId, exception)));
                else
                    throw new DataTransmissionException("An error occurred while transmitting metadata.",
                        brandCode, correlationId, exception);
            }
        } // todo: Auto-swallow exceptions

        public async Task TransmitAsync( // todo: Create as custom attribute/middleware
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

        private void OnShutdownFailed(ShutdownFailedEventArgs e)
        {
            ShutdownFailed?.Invoke(this, e);
        }
    }
}