using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
using Grpc.Core;
using Newtonsoft.Json;

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

            var serviceCredentials = new GcpServiceCredentials
            {
                Type = "service_account",
                ProjectId = "eshop-bigdata",
                PrivateKeyId = "803e08690482a290b7a52b45a6205a7bcaf514b1", // todo: Enter appropriate value
                PrivateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvQIBADANBgkqhkiG9w0BAQEFAASCBKcwggSjAgEAAoIBAQC9BjNqrPhu6leU\neR9CN/q2u6pqNgXyJ+1lVhXrb+cdVxrhc+2ox96giUG1IMIu+dBQ/833s1rueozH\nllccdiSFPLF8BxYheanQtHgDxsorMq3JQnw79wJRrSsKa3BifIsbuJVd31IhlCI8\n5rVXGKFraInjLLJpdgDliXt0b/f5ckxC7SfsGKOkqOfqsZ36QFhVCDsOOXfnTn34\n0ZHWUCAzpf1UyxdykDjmpLxp+vnOd+ALFetyTgzfmvkwt1/4L0w6pAVvocWGFlEF\ny+unkfKpFblyrBFvfqAlRoNRBIztjzWcG2r2RL+sLl2F4XttMhNZdV7KzkWIB1kD\nNuG9h8XJAgMBAAECggEABEyITdRKQCeHP2Kz0cs5If/Jo+m/n97FmOjozbZ5UKMD\nAv6ieJS3HPqSKn7ou63FzZnLwubq2gKiggWKzKRpNziGptX4bGF6ebspAZj7Txkz\nd6DR94EqJdZk5LNF1o+TkFlV2FE0anwWfUEX/bebMDir7iU9l4VuBspmKuNDBgLH\n5/ZP1afxapPlr1Mtyvb/kSR3ateSxwM9sr1AxdA8iaZ2H0iGpFVJArkCsCgvDBt9\nyGNoezmSw+XbH/TCybcRFWfS9sao3MpvMblG1LWX8UWaj/zqa73k6Q1zmO+BOe4w\nPnhReJpAFk6W7LbC/S9r7bBywGsvyng1cNh5TmjicwKBgQDzCF4vpJp1eI3jZpR5\npqdouYHoRP6AzXbyuow8wPBLPtEEdWGPTLjEIq67Jg2WkjkmSPAN5Ai6iI8MaJbV\nsJVh7hPECRPDJ07XSZG2KXiLrBTwkWGjJgkeQ/8XKOtsHhkrKZUbV2D1H+bXPfeh\nOKOE7Kvkys9zYDijsUSy84ALRwKBgQDHHB7ZXnXWfPKyNSFUZKMQY3Dt/8OTS/25\nGuXVt1ZTNyEZNNWqAfUcT4IACMenB0CMQ54zqvHDye3zKz/B7qhKknvA00EKG5J9\n6oUrnnzobaqBrAz1WEL8Q6HTUIdd4VaUbMKbYMM2DnBcwAYSNNRw3Tiyna0zmWxO\n8fMv7PMObwKBgQCpYqKqmBHjX/Ock4lWRFYXwnuNVFEBmrKVuHk3es2/0/dAIUOu\n6a05XmbkH0CHOkDEsz9EnzOKNtVks5y1MQ7co195WU5BzSrBGBCLotnWl0g850bi\nvAbM5l1rWeTDhTLLh37aAIueLO7qA9GMt3oYkg+4NbZi1qSDSnD0PIx4zwKBgHx0\nM0QjKvy6dOi3FPIvRU2FGp1o9NIo+ZquGWMQicDSALpEsBjnyFG90MA8vK7Gda42\nxbf97cg3e6g3LE8H4eFa1kecxFaRDWvvHvY4xlJx2dXbuO3SEWykyY3QFAy2QOvd\nX3bHcL1lIQ6YaDMRGojBfiMHM6/BAlYGHb+jF/m5AoGAQ1DBjLuvWL9lNe5l6TY3\nNcsqLqtfXb6phcCQZtDnx7ejawCT4NIspHPXr64WaSbrLNg6DIes/aeRlzJ++MY2\nDmbF8Xki0k/BRzdVcDpyerx1sFju7DFrZSLOHUwfE5A8vtYahc1TtImXWItKl1P2\n3JXHXk+9DjjDF6p4XSyzlgQ=\n-----END PRIVATE KEY-----\n", // todo: Enter appropriate value
                ClientEmail = "pub-sub-integration-tests@eshop-bigdata.iam.gserviceaccount.com", // todo: Enter appropriate value
                ClientId = "106075234704991058047", // todo: Enter appropriate value
                AuthUri = "https://accounts.google.com/o/oauth2/auth",
                TokenUri = "https://oauth2.googleapis.com/token",
                AuthProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs",
                ClientX509CertUrl = "https://www.googleapis.com/robot/v1/metadata/x509/pub-sub-integration-tests%40eshop-bigdata.iam.gserviceaccount.comStar" // todo: Enter appropriate value
            };

            var jsonServiceCredentials = JsonConvert.SerializeObject(serviceCredentials);
            BootUp(jsonServiceCredentials, gcpProjectId, pubSubTopicId);

            var subscriberCredential = GoogleCredential.FromJson(jsonServiceCredentials)
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
            await subscriber.StopAsync(CancellationToken.None);
        }
    }
}