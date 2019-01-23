using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshopworld.Strada.Plugins.Streaming;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.PubSub.V1;
using Grpc.Auth;
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
                PrivateKeyId = "{PRIVATE KEY ID}", // todo: Enter appropriate value
                PrivateKey = "{PRIVATE KEY}", // todo: Enter appropriate value
                ClientEmail = "{CLIENT EMAIL}", // todo: Enter appropriate value
                ClientId = "{CLIENT ID}", // todo: Enter appropriate value
                AuthUri = "https://accounts.google.com/o/oauth2/auth",
                TokenUri = "https://oauth2.googleapis.com/token",
                AuthProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs",
                ClientX509CertUrl = "{CLIENT X509 CERT URL}" // todo: Enter appropriate value
            };

            var jsonServiceCredentials = JsonConvert.SerializeObject(serviceCredentials);

            var credential = GoogleCredential
                .FromJson(jsonServiceCredentials)
                .CreateScoped(SubscriberServiceApiClient.DefaultScopes);

            var clientCreationSettings =
                new SubscriberClient.ClientCreationSettings(null, null, credential.ToChannelCredentials());

            var subscriptionName = new SubscriptionName(gcpProjectId, pubSubTopicId);
            var subscriber = await SubscriberClient.CreateAsync(subscriptionName, clientCreationSettings);

            Console.WriteLine(@"Receiving transmissions ...");
            Console.WriteLine(string.Empty);

            var ordersMeta = new List<bool>();

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            await subscriber.StartAsync(async (msg, cancellationToken) =>
            {
                var data = msg.Data.ToStringUtf8();
                var order = JsonConvert.DeserializeObject<Order>(data);

                ordersMeta.Add(order.Complete);

                var completeOrders = Convert.ToInt64(ordersMeta.Count(o => o));
                var orderConversion = completeOrders / (double) ordersMeta.Count;

                var orderConversionRate = orderConversion * 100;
                Console.WriteLine(orderConversionRate + "%");

                return await Task.FromResult(SubscriberClient.Reply.Ack);
            });

            Console.ReadLine();
            await subscriber.StopAsync(TimeSpan.Zero);
        }
    }
}