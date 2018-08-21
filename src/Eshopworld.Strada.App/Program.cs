using System;
using System.Collections.Generic;
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

        private static async Task MainAsync(IReadOnlyList<string> args)
        {
            var gcpProjectId = args[0];
            var pubSubTopicId = args[1];

            var client = new HttpClient();
            var serviceCredentials = client.GetStringAsync(Resources.CredentialsFileUri).Result;
            Console.WriteLine(@"Service credentials downloaded ...");

            BootUp(serviceCredentials, gcpProjectId, pubSubTopicId);

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

            await PullMessages(subscriber, true);

            Console.WriteLine(@"All transmissions received.");
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

        private static async Task PullMessages(SubscriberClient subscriber, bool acknowledge)
        {
            await subscriber.StartAsync(
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
            Thread.Sleep(3000);
            subscriber.StopAsync(CancellationToken.None).Wait();
        }
    }
}