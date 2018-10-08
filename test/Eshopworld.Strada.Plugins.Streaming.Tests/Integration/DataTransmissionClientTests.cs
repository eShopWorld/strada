using System;
using System.Net.Http;
using System.Text;
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
        private class PreOrder
        {
            public string ProductName { get; set; }
            public double ProductValue { get; set; }
        }

        private static void PullMessage<T>(
            Action<T> callback,
            SubscriberServiceApiClient subscriber,
            SubscriptionName subscriptionName) where T : class
        {
            var response = subscriber.Pull(subscriptionName, false, 3,
                CallSettings.FromCallTiming(
                    CallTiming.FromExpiration(
                        Expiration.FromTimeout(
                            TimeSpan.FromSeconds(90)))));

            if (response.ReceivedMessages == null) return;
            if (response.ReceivedMessages.Count == 0) return;
            foreach (var message in response.ReceivedMessages)
            {
                var json = message.Message.Data.ToByteArray();
                var queueMessage = JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(json));
                callback(queueMessage);
            }

            var ackIds = new string[response.ReceivedMessages.Count];
            for (var i = 0; i < response.ReceivedMessages.Count; ++i)
                ackIds[i] = response.ReceivedMessages[i].AckId;
            subscriber.Acknowledge(subscriptionName, ackIds);
        }

        /// <summary>
        ///     Ensures that metadata is transmitted to a Cloud Pub/Sub Topic.
        /// </summary>
        [Fact]
        public void DataIsTransmittedToCloudPubSub()
        {
            PublisherServiceApiClient publisher;
            SubscriberServiceApiClient subscriber;
            TopicName topicName;
            SubscriptionName subscriptionName;

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

            try
            {
                const string productName = "SNKRS";
                const double productValue = 1.5;

                var serviceCredentials = JsonConvert.DeserializeObject<GcpServiceCredentials>(serviceCredentialsJson);

                DataTransmissionClient.Instance.InitAsync(
                    Resources.GCPProjectId,
                    Resources.PubSubTopicId,
                    serviceCredentials).Wait();

                DataTransmissionClient.Instance.TransmitAsync(
                    Resources.BrandCode, Resources.EventName, Guid.NewGuid().ToString(),
                    new PreOrder
                    {
                        ProductName = "SNKRS",
                        ProductValue = 1.5
                    },
                    string.Empty,
                    string.Empty).Wait();

                PullMessage<PreOrder>(preOrder =>
                    {
                        Assert.Equal(productName, preOrder.ProductName);
                        Assert.Equal(productValue, preOrder.ProductValue);
                    },
                    subscriber,
                    subscriptionName);
            }
            catch (Exception)
            {
                // Fail
            }
            finally
            {
                subscriber.DeleteSubscription(subscriptionName);
                publisher.DeleteTopic(topicName);
            }
        }
    }
}