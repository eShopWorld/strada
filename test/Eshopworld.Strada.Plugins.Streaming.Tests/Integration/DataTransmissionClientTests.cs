using System;
using System.Collections.Generic;
using System.Net.Http;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Integration
{
    public class DataTransmissionClientTests
    {
        private static CloudServiceCredentials Init(
            out SubscriberServiceApiClient subscriber,
            out SubscriptionName subscriptionName,
            out PublisherServiceApiClient publisher,
            out TopicName topicName,
            out DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            CloudServiceCredentials gcpServiceCredentials;

            try
            {
                topicName = new TopicName(Resources.GCPProjectId, Resources.PubSubTopicId);
                subscriptionName = new SubscriptionName(Resources.GCPProjectId, Resources.PubSubSubscriptionId);

                using (var client = new HttpClient())
                {
                    gcpServiceCredentials =
                        JsonConvert.DeserializeObject<CloudServiceCredentials>(client
                            .GetStringAsync(Resources.CredentialsFileUri).Result);

                    dataTransmissionClientConfigSettings =
                        JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(client
                            .GetStringAsync(Resources.ConfigFileUri).Result);
                }

                var publisherCredential = GoogleCredential.FromJson(JsonConvert.SerializeObject(gcpServiceCredentials))
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);
                var publisherChannel = new Channel(
                    PublisherServiceApiClient.DefaultEndpoint.ToString(),
                    publisherCredential.ToChannelCredentials());
                publisher = PublisherServiceApiClient.Create(publisherChannel);

                var subscriberCredential = GoogleCredential.FromJson(JsonConvert.SerializeObject(gcpServiceCredentials))
                    .CreateScoped(SubscriberServiceApiClient.DefaultScopes);
                var subscriberChannel = new Channel(
                    SubscriberServiceApiClient.DefaultEndpoint.ToString(),
                    subscriberCredential.ToChannelCredentials());
                subscriber = SubscriberServiceApiClient.Create(subscriberChannel);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to initialize Pub/Sub Topic or Subscription.", exception);
            }

            try
            {
                publisher.CreateTopic(topicName);
            }
            catch (RpcException e)
                when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Topic already exists. 
            }

            try
            {
                subscriber.CreateSubscription(subscriptionName, topicName, null, 0);
            }
            catch (RpcException e)
                when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Subscription already exists.
            }

            return gcpServiceCredentials;
        }

        private static void PullMessage(
            Action callback,
            SubscriberServiceApiClient subscriber,
            SubscriptionName subscriptionName)
        {
            var response = subscriber.Pull(subscriptionName, false, 10,
                CallSettings.FromCallTiming(
                    CallTiming.FromExpiration(
                        Expiration.FromTimeout(
                            TimeSpan.FromSeconds(90)))));

            if (response.ReceivedMessages == null) return;
            if (response.ReceivedMessages.Count == 0) return;
            foreach (var unused in response.ReceivedMessages) callback();

            var ackIds = new string[response.ReceivedMessages.Count];
            for (var i = 0; i < response.ReceivedMessages.Count; ++i)
                ackIds[i] = response.ReceivedMessages[i].AckId;
            subscriber.Acknowledge(subscriptionName, ackIds);
        }

        [Fact]
        public void DataIsTransmittedToCloudPubSub()
        {
            PublisherServiceApiClient publisher = null;
            SubscriberServiceApiClient subscriber = null;
            TopicName topicName = null;
            SubscriptionName subscriptionName = null;

            try
            {
                var gcpServiceCredentials = Init(
                    out subscriber,
                    out subscriptionName,
                    out publisher,
                    out topicName,
                    out var dataTransmissionClientConfigSettings);

                var eventMetadataCache = new EventMetaCache();
                var httpHeaders = new Dictionary<string, string>
                    {{"User-Agent", "USERAGENT"}, {"Content-Type", "CONTENT"}};

                for (var i = 0; i < 10; i++)
                    eventMetadataCache.Add(
                        new SimpleObject {Name = "TEST"},
                        "BRAND",
                        "TEST",
                        Guid.NewGuid().ToString(),
                        "QUERY",
                        httpHeaders);

                var dataTransmissionClient = new DataTransmissionClient();

                dataTransmissionClient.InitAsync(
                    gcpServiceCredentials,
                    dataTransmissionClientConfigSettings).Wait();

                dataTransmissionClient.TransmitAsync(eventMetadataCache.GetEventMetadataPayloadBatch()).Wait();

                var counter = 0;
                PullMessage(() => { counter++; },
                    subscriber,
                    subscriptionName);

                Assert.Equal(10, counter);
            }
            finally
            {
                subscriber?.DeleteSubscription(subscriptionName);
                publisher?.DeleteTopic(topicName);
            }
        }
    }
}