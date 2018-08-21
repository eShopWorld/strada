using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;

namespace Eshopworld.Strada.App
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            //if (args == null || args.Length.Equals(0))
            //    throw new ArgumentNullException(nameof(args));
            var gcpProjectId = args[0];
            var pubSubTopicId = args[1];
            //var jsonFilePath = args[2];
            //var brandCode = args[3];
            //var correlationId = args[4];

            var client = new HttpClient();
            var serviceCredentials = client.GetStringAsync(Resources.CredentialsFileUri).Result;
            Console.WriteLine(@"Service credentials downloaded ...");

            //BootUp(serviceCredentials, gcpProjectId, pubSubTopicId);
            //Console.WriteLine(@"Boot-up complete ...");

            //try
            //{
            //    if (!File.Exists(jsonFilePath))
            //        throw new FileNotFoundException("File not found.");
            //    var json = await File.ReadAllTextAsync(jsonFilePath);
            //    Console.WriteLine(@"JSON extracted ...");

            //    DataTransmissionClient.Instance.Init(
            //        gcpProjectId,
            //        pubSubTopicId,
            //        serviceCredentials);
            //    Console.WriteLine(@"Transmission client initialised ...");

            //    Order order = new Order();
            //    order.OrderNumber = Guid.NewGuid().ToString();
            //    order.Value = 200.99m;

            //    await DataTransmissionClient.Instance.TransmitAsync(brandCode, correlationId, order);
            //    Console.WriteLine(@"Data transmission complete ...");
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception.Message);
            //    return;
            //} // todo: Create simple ASP.NET Core app ...

            //await DataTransmissionClient.ShutDownAsync();
            //Console.WriteLine(@"Shutdown complete. Press any key.");

            var subscriberCredential = GoogleCredential.FromJson(serviceCredentials)
                .CreateScoped(SubscriberServiceApiClient.DefaultScopes);
            var subscriberChannel = new Channel(
                SubscriberServiceApiClient.DefaultEndpoint.ToString(),
                subscriberCredential.ToChannelCredentials());
            var subscriberClient = SubscriberServiceApiClient.Create(subscriberChannel);

            var subscriptionName = new SubscriptionName(
                gcpProjectId,
                pubSubTopicId);

            var subscriber = SubscriberClient.Create(
                subscriptionName, new[] {subscriberClient},
                new SubscriberClient.Settings
                {
                    AckExtensionWindow = TimeSpan.FromSeconds(4),
                    Scheduler = SystemScheduler.Instance,
                    StreamAckDeadline = TimeSpan.FromSeconds(10),
                    FlowControlSettings = new FlowControlSettings(
                        100,
                        10240)
                });

            PullMessages(subscriber, true);

            Console.ReadLine();
        }

        private static void BootUp(string serviceCredentials, string gcpProjectId, string pubSubTopicId)
        {
            PublisherServiceApiClient publisher;
            TopicName topicName;

            try
            {
                topicName = new TopicName(gcpProjectId, pubSubTopicId);

                var publisherCredential = GoogleCredential.FromJson(serviceCredentials)
                    .CreateScoped(PublisherServiceApiClient.DefaultScopes);
                var publisherChannel = new Channel(
                    PublisherServiceApiClient.DefaultEndpoint.ToString(),
                    publisherCredential.ToChannelCredentials());
                publisher = PublisherServiceApiClient.Create(publisherChannel);
            }
            catch (Exception exception)
            {
                throw new Exception("Failed to initialize Pub/Sub Topic.", exception);
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
        }

        public static void PullMessages(SubscriberClient subscriber, bool acknowledge)
        {
            // [START pubsub_quickstart_subscriber]
            // [START pubsub_subscriber_flow_settings]
            // SimpleSubscriber runs your message handle function on multiple
            // threads to maximize throughput.
            subscriber.StartAsync(
                async (message, cancel) =>
                {
                    var text =
                        Encoding.UTF8.GetString(message.Data.ToArray());
                    await Console.Out.WriteLineAsync(
                        $"Message {message.MessageId}: {text}");
                    return acknowledge
                        ? SubscriberClient.Reply.Ack
                        : SubscriberClient.Reply.Nack;
                });
            // Run for 3 seconds.
            Thread.Sleep(3000);
            subscriber.StopAsync(CancellationToken.None).Wait();
            // [END pubsub_subscriber_flow_settings]
            // [END pubsub_quickstart_subscriber]
        }
    }

    internal class Order
    {
        public string OrderNumber { get; set; }
        public decimal Value { get; set; }
    }
}