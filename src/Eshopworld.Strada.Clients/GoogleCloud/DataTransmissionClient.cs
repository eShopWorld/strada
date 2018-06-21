using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Clients.GoogleCloud
{
    /// <summary>
    ///     DataTransmissionClient is a static Cloud Pub/Sub client, providing connectivity and transmission functionality.
    /// </summary>
    public class DataTransmissionClient
    {
        private static readonly Lazy<DataTransmissionClient> InnerDataTransmissionClient =
            new Lazy<DataTransmissionClient>(() => new DataTransmissionClient());

        private PublisherServiceApiClient _publisher;
        private TopicName _topicName;

        /// <summary>
        ///     Instance is a static instance of <see cref="DataTransmissionClient" />.
        /// </summary>
        public static DataTransmissionClient Instance => InnerDataTransmissionClient.Value;

        /// <summary>
        ///     Init instantiates Cloud Pub/Sub connectivity metadata.
        /// </summary>
        /// <param name="projectId">The Cloud Pub/Sub Project ID.</param>
        /// <param name="topicId">The Cloud Pub/Sub Topic ID</param>
        public void Init(string projectId, string topicId)
        {
            const string credentialsFilePath = "Content/data-analytics-421f476fd5e8.json";

            var publisherCredential = GoogleCredential.FromFile(credentialsFilePath)
                .CreateScoped(PublisherServiceApiClient.DefaultScopes);
            var publisherChannel = new Channel(
                PublisherServiceApiClient.DefaultEndpoint.ToString(),
                publisherCredential.ToChannelCredentials());

            _publisher = PublisherServiceApiClient.Create(publisherChannel);
            _topicName = new TopicName(projectId, topicId);
        }

        /// <summary>
        ///     Transmit persists <see cref="metadata" /> with associated <see cref="brand" /> metadata
        ///     to the connected Cloud Pub/Sub instance.
        /// </summary>
        /// <param name="metadata">The metadata to transmit to the Cloud Pub/Sub instance.</param>
        /// <param name="brand">The brand associated with <see cref="metadata" />.</param>
        public async Task Transmit(object metadata, string brand = null)
        {
            // Todo: Handle lost connectivity with RetryPolicy.
            var payload = JsonConvert.SerializeObject(metadata);
            await _publisher.PublishAsync(_topicName, new[]
            {
                new PubsubMessage
                {
                    Data = ByteString.CopyFromUtf8(payload)
                }
            });
        }
    }
}