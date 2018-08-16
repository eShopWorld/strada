using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;

namespace Eshopworld.Strada.App
{
    internal static class Program
    {
        private static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            var client = new HttpClient();
            var serviceCredentials = client.GetStringAsync(Resources.CredentialsFileUri).Result;
            Console.WriteLine(@"Service credentials downloaded ...");

            BootUp(serviceCredentials);
            Console.WriteLine(@"Boot-up complete ...");

            DataTransmissionClient.Instance.Init(
                Resources.GCPProjectId, // Change this resource entry to match your GCP project ID
                Resources.PubSubTopicId, // Change this resource entry to match your GCP Pub/Sub instance
                serviceCredentials);
            Console.WriteLine(@"Transmission client initialised ...");

            try
            {
                await DataTransmissionClient.Instance.TransmitAsync(
                    Resources.BrandCode, Guid.NewGuid().ToString(),
                    new PreOrder
                    {
                        Number = Guid.NewGuid().ToString(),
                        Value = 150.99m,
                        CreatedDate = DateTime.UtcNow,
                        Addresses = new List<AddressDetails>
                        {
                            new AddressDetails
                            {
                                Status = "Current",
                                Address = "1 New Road",
                                City = "Dublin",
                                State = "Leinster",
                                Zip = "DUB-12345"
                            },
                            new AddressDetails
                            {
                                Status = "Previous",
                                Address = "2 New Road",
                                City = "Dublin",
                                State = "Leinster",
                                Zip = "DUB-12345"
                            }
                        }
                    });
                Console.WriteLine(@"Data transmission complete ...");
            }
            catch (DataTransmissionException exception)
            {
                Console.WriteLine(exception.BrandCode);
                Console.WriteLine(exception.CorrelationId);
                Console.WriteLine(exception.Message);
            }

            await DataTransmissionClient.ShutDownAsync();
            Console.WriteLine(@"Shutdown complete. Press any key.");
            Console.ReadLine();
        }

        private static void BootUp(string serviceCredentials)
        {
            PublisherServiceApiClient publisher;
            SubscriberServiceApiClient subscriber;
            TopicName topicName;
            SubscriptionName subscriptionName;

            try
            {
                topicName = new TopicName(Resources.GCPProjectId, Resources.PubSubTopicId);
                subscriptionName = new SubscriptionName(Resources.GCPProjectId, Resources.PubSubSubscriptionId);

                var publisherCredential = GoogleCredential.FromJson(serviceCredentials)
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);
                var publisherChannel = new Channel(
                    PublisherServiceApiClient.DefaultEndpoint.ToString(),
                    publisherCredential.ToChannelCredentials());
                publisher = PublisherServiceApiClient.Create(publisherChannel);

                var subscriberCredential = GoogleCredential.FromJson(serviceCredentials)
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
}