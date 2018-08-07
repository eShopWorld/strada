using System;
using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;

namespace Eshopworld.Strada.App
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            BootUp();

            DataTransmissionClient.Instance.Init(
                Resources.GCPProjectId,
                Resources.PubSubTopicId,
                "Content/data-analytics-421f476fd5e8.json");

            DataTransmissionClient.Instance.TransmissionFailed += Instance_TransmissionFailed;

            await DataTransmissionClient.Instance.TransmitAsync(
                Resources.BrandName,
                new PreOrder
                {
                    ProductName = "SNKRS",
                    ProductValue = 1.5
                });
        }

        private static void Instance_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
        }

        // todo: create shutdown method
        private static void BootUp()
        {
            PublisherServiceApiClient publisher;
            SubscriberServiceApiClient subscriber;
            TopicName topicName;
            SubscriptionName subscriptionName;

            const string credentialsFilePath = "Content/data-analytics-421f476fd5e8.json";

            try
            {
                topicName = new TopicName(Resources.GCPProjectId, Resources.PubSubTopicId);
                subscriptionName = new SubscriptionName(Resources.GCPProjectId, Resources.PubSubSubscriptionId);

                var publisherCredential = GoogleCredential.FromFile(credentialsFilePath)
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);
                var publisherChannel = new Channel(
                    PublisherServiceApiClient.DefaultEndpoint.ToString(),
                    publisherCredential.ToChannelCredentials());
                publisher = PublisherServiceApiClient.Create(publisherChannel);

                var subscriberCredential = GoogleCredential.FromFile(credentialsFilePath)
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
        }
    }

    internal class PreOrder
    {
        public string ProductName { get; set; }
        public double ProductValue { get; set; }
    }
}