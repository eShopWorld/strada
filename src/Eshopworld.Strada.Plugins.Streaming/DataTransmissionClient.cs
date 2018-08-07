﻿using System;
using System.Threading.Tasks;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public delegate void TransmissionFailedEventHandler(object sender, TransmissionFailedEventArgs e);

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
        ///     Raised when and exception occurs during <see cref="TransmitAsync{T}" /> execution.
        /// </summary>
        public event TransmissionFailedEventHandler TransmissionFailed;

        /// <summary>
        ///     Init instantiates Cloud Pub/Sub connectivity components.
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
            // todo: shutdown publisherChannel
            _publisher = PublisherServiceApiClient.Create(publisherChannel);
            _topicName = new TopicName(projectId, topicId);
        }

        /// <summary>
        ///     TransmitAsync persists <see cref="metadata" /> with associated <see cref="brandName" /> metadata
        ///     to the connected Cloud Pub/Sub instance.
        /// </summary>
        /// <param name="brandName">The brand name associated with <see cref="metadata" />.</param>
        /// <param name="metadata">The metadata to transmit to the Cloud Pub/Sub instance.</param>
        public async Task TransmitAsync<T>(string brandName, T metadata) where T : class
        {
            if (string.IsNullOrEmpty(brandName)) throw new ArgumentNullException(nameof(brandName));
            if (metadata == null) throw new ArgumentNullException(nameof(metadata));

            try
            {
                var metadataWrapper = new MetadataWrapper<T>
                {
                    BrandName = brandName,
                    Metadata = metadata
                };

                var payload = JsonConvert.SerializeObject(metadataWrapper);
                await _publisher.PublishAsync(_topicName, new[]
                {
                    new PubsubMessage
                    {
                        Data = ByteString.CopyFromUtf8(payload)
                    }
                }, CallSettings.FromCallTiming(CallTiming.FromTimeout(TimeSpan.FromSeconds(3)))); // introduce as parameter.
            }
            catch (Exception exception)
            {
                OnTransmissionFailed(new TransmissionFailedEventArgs(exception));
            }
        }

        private void OnTransmissionFailed(TransmissionFailedEventArgs e)
        {
            TransmissionFailed?.Invoke(this, e);
        }
    }
}