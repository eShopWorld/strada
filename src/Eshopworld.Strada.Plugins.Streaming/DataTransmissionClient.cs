using System;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming
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
        /// <param name="credentialsFilePath">The GCP Pub/Sub credentials file path.</param>
        public void Init(string projectId, string topicId, string credentialsFilePath)
        {
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
        /// <param name="brand">The brand associated with <see cref="metadata" />.</param>
        /// <param name="metadata">The metadata to transmit to the Cloud Pub/Sub instance.</param>
        public async Task Transmit(string brand, object metadata)
        {
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