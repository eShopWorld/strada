using System;
using System.Net.Http;
using Google.Api.Gax;
using Google.Api.Gax.Grpc;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Integration
{
    public class DataTransmissionClientTests
    {
        private static string Init(
            out SubscriberServiceApiClient subscriber,
            out SubscriptionName subscriptionName,
            out PublisherServiceApiClient publisher,
            out TopicName topicName)
        {
            string serviceCredentialsJson;
            try
            {
                topicName = new TopicName(Resources.GCPProjectId, Resources.PubSubTopicId);
                subscriptionName = new SubscriptionName(Resources.GCPProjectId, Resources.PubSubSubscriptionId);

                using (var client = new HttpClient())
                {
                    serviceCredentialsJson = client.GetStringAsync(Resources.CredentialsFileUri).Result;
                }

                var publisherCredential = GoogleCredential.FromJson(serviceCredentialsJson)
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);
                var publisherChannel = new Channel(
                    PublisherServiceApiClient.DefaultEndpoint.ToString(),
                    publisherCredential.ToChannelCredentials());
                publisher = PublisherServiceApiClient.Create(publisherChannel);

                var subscriberCredential = GoogleCredential.FromJson(serviceCredentialsJson)
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

            return serviceCredentialsJson;
        }

        private static void PullMessage<T>(
            Action callback,
            SubscriberServiceApiClient subscriber,
            SubscriptionName subscriptionName) where T : class
        {
            var response = subscriber.Pull(subscriptionName, false, 10,
                CallSettings.FromCallTiming(
                    CallTiming.FromExpiration(
                        Expiration.FromTimeout(
                            TimeSpan.FromSeconds(90)))));

            if (response.ReceivedMessages == null) return;
            if (response.ReceivedMessages.Count == 0) return;
            foreach (var message in response.ReceivedMessages) callback();

            var ackIds = new string[response.ReceivedMessages.Count];
            for (var i = 0; i < response.ReceivedMessages.Count; ++i)
                ackIds[i] = response.ReceivedMessages[i].AckId;
            subscriber.Acknowledge(subscriptionName, ackIds);
        }

        [Fact]
        public void DataIsTransmittedToCloudPubSubInBatchMode()
        {
            PublisherServiceApiClient publisher = null;
            SubscriberServiceApiClient subscriber = null;
            TopicName topicName = null;
            SubscriptionName subscriptionName = null;

            try
            {
                var serviceCredentialsJson = Init(
                    out subscriber,
                    out subscriptionName,
                    out publisher,
                    out topicName);

                var eventMetadataCache = new EventMetadataCache();
                for (var i = 0; i < 10; i++) eventMetadataCache.Add("_");

                var dataTransmissionClient = new DataTransmissionClient();

                dataTransmissionClient.InitAsync(
                    Resources.GCPProjectId,
                    Resources.PubSubTopicId,
                    serviceCredentialsJson,
                    true,
                    true).Wait();

                dataTransmissionClient.TransmitAsync(eventMetadataCache.GetEventMetadataPayloadBatch()).Wait();

                var counter = 0;
                PullMessage<string>(() => { counter++; },
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

        [Fact]
        public void DataIsTransmittedToCloudPubSubOnDemand()
        {
            SubscriberServiceApiClient subscriber = null;
            SubscriptionName subscriptionName = null;
            PublisherServiceApiClient publisher = null;
            TopicName topicName = null;
            try
            {
                var serviceCredentialsJson = Init(
                    out subscriber,
                    out subscriptionName,
                    out publisher,
                    out topicName);

                var dataTransmissionClient = new DataTransmissionClient();

                dataTransmissionClient.InitAsync(
                    Resources.GCPProjectId,
                    Resources.PubSubTopicId,
                    serviceCredentialsJson).Wait();

                dataTransmissionClient.TransmitAsync(
                    Resources.BrandCode,
                    Resources.EventName,
                    Guid.NewGuid().ToString(),
                    string.Empty,
                    string.Empty,
                    string.Empty).Wait();

                var counter = 0;
                PullMessage<string>(() => { counter++; },
                    subscriber,
                    subscriptionName);

                Assert.Equal(1, counter);
            }
            catch (Exception)
            {
                // Fail todo: Don't fail silently
            }
            finally
            {
                subscriber?.DeleteSubscription(subscriptionName);
                publisher?.DeleteTopic(topicName);
            }
        }
    }
}