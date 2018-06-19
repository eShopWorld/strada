using System;
using Eshopworld.Strada.Clients.GoogleCloud;
using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration.GoogleCloud
{
    public class DataTransmissionClientTests
    {
        /// <summary>
        ///     Ensures that metadata is transmitted to a Cloud Pub/Sub Topic.
        /// </summary>
        [Fact]
        public void DataIsTransmittedToCloudPubSub()
        {
            PublisherClient pub;
            SubscriberClient sub;
            TopicName topicName;
            SubscriptionName subscriptionName;

            try
            {
                topicName = new TopicName(Resources.GCPProjectId, Resources.PubSubTopicId);
                subscriptionName = new SubscriptionName(Resources.GCPProjectId, Resources.PubSubSubscriptionId);

                pub = PublisherClient.Create();
                sub = SubscriberClient.Create();
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to initialize Pub/Sub Topic or Subscription.", exception);
            }

            try
            {
                pub.CreateTopic(topicName);
            }
            catch (RpcException e)
                when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Topic already exists. 
            }

            try
            {
                sub.CreateSubscription(subscriptionName, topicName, null, 0);
            }
            catch (RpcException e)
                when (e.Status.StatusCode == StatusCode.AlreadyExists)
            {
                // Subscription already exists.
            }

            var dataTransmissionClient = new DataTransmissionClient();

            try
            {
                dataTransmissionClient.Init(Resources.GCPProjectId, Resources.PubSubTopicId);
                dataTransmissionClient.Transmit(string.Empty, string.Empty).Wait();
            }
            finally
            {
                sub.DeleteSubscription(subscriptionName);
                pub.DeleteTopic(topicName);
            }
        }
    }
}