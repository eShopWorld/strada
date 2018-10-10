using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
                PrivateKeyId = "c475eb5893123613f11fe7b9a98e1e440f3132e2", // todo: Enter appropriate value
                PrivateKey =
                    "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCKntOs8jSD5xj1\nzAdFKsjoFea+CnmZSVl/acKTBZ1g5BPxPorkI4FZ+dFN61FAkJ1God3sxePHfwlF\nJTnfNWjeUxFYcdYOVmtQ6+JQdEbGzOY59nMJGMfcDnZbc1ooRrXAQHqR3aQ7lB5z\np8RSzCpDg2nRJEforx2hm6brnrmfdfZ2dc57ocQttSsrHm9N6hbXmZdCgEafEmLq\nf8FLZzN2qWYcSqK+B/cpWrrjv5lHYkvokCFMPVenEJmpti2nSiNsiketNwXS5hsG\ngBKHKPEKgQhTqz7FpFU8T0unPd0W7DVekpkOmgPx0ZT/3uTw4JRRcz0vXCsvdGu3\nw3mW4fibAgMBAAECggEAJ5NRvpoFqTTUuPQPjU06P5xlHJOUBNHepdQ1c1ESjeN/\n2AuxjpGeb+g1O+g3cdX3uP8kg2E59y5LIL0QiSvP7U1M8fHKGZzNlbRKkAqqXD6K\najj0vb+f67ELVG+Z6U7KvAEMnVFofoP6r61RjQvx8wV1M/sGNqWsMYPlQ3a06xPQ\ngLcZaxbZe2lpArdNIUgW8AlRrRwOcXOU/q+m1OqGFZTDGOJOHOfOVumZXKy3Gdk3\nSlzDoqsdXxuki42flseiXUfq4wHjkKEhQRyJwqG7HV4x8s7RTFUqpFJWegDiw1eg\nXQJ2K1c7CSILIbumRjA+REzF9P7D3vrsUOn7RgZdyQKBgQC953fQi1cUfbSulhT5\nGbft/eBCcpgOEP5lVeBs10Egi92KupNvCmC1CLhLKdncv94tb+ABC4sod1rWm7ni\nUndYXkSvXRstH3HsL7wiFNoDtYqEVawghX9Wea7+FwGj1H91k83ZlPsmE3heclbe\nK3gSS/XGJg8VyLgNOtzAaW5uaQKBgQC63fUTgYfwuNlQ0Wz849EtbOm/4gFjdk4U\nMYjBDE2aBSu38yNFQMrx4q67oYrH0Pbj0KEaXszZrTrcH99lRicwiZeVP8g9nQ2x\nBzd8n+58Klh7e1lNerRib1faFNL2k4w+IZkQsfANK20Dr4cVEHmjeUZZiK+QELnK\n9Ja1IKZWYwKBgGvqL0IJDLEORjC4BK8RmF3b2SapbBMCQS5gwKnZIJ9YG8sL/2Ao\na5A0plXAMJerSJxVaNvvLWMPgEVYNSeRaVSELU/h/uGDbv+imKxdYQ6eiVpuPOQ2\nOIhxmam2dS4eQVgVZ/LvIFEg21QAcbDKzu7Gz6GMWyIr77tE6dFFmIWRAoGABjul\nExN8/1CLHyD9K6pFreg2G8pkFXc6v32vfBD2/a4yeR5JDOROYKcZAPUwdd3IfmzV\nEtYaqyAWGIWPpAclA39zaO0JttRoQoFlHmkPQWEANo40ulrfbXEdUw7iworFYMUq\nH95vjWDnb/oI3XEBPayr5gyjHoEQUW67ICuIoNUCgYBEVdtHXfD8HuAT94f+nB1L\nM5Czf7y4/DuTpnoE9GbeBEIkCDn0BkLS6KcrE9c6FGYP0/fVuuFvVkdht/xBW8Mi\nLM3eO8U1YEoaYXXkXA8b98hUr3oxejPvu+aI4svGXS/QH4va6v9+jNaxXl3YPBHi\nvckcX/r/18qSnIX5c2uSIQ==\n-----END PRIVATE KEY-----\n", // todo: Enter appropriate value
                ClientEmail = "dataflow-runner@eshop-bigdata.iam.gserviceaccount.com", // todo: Enter appropriate value
                ClientId = "103689787570041306234", // todo: Enter appropriate value
                AuthUri = "https://accounts.google.com/o/oauth2/auth",
                TokenUri = "https://oauth2.googleapis.com/token",
                AuthProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs",
                ClientX509CertUrl =
                    "https://www.googleapis.com/robot/v1/metadata/x509/dataflow-runner%40eshop-bigdata.iam.gserviceaccount.com" // todo: Enter appropriate value
            };

            var jsonServiceCredentials = JsonConvert.SerializeObject(serviceCredentials);

            var credential = GoogleCredential
                .FromJson(jsonServiceCredentials)
                .CreateScoped(SubscriberServiceApiClient.DefaultScopes);

            var receivedMessages = new List<PubsubMessage>();
            var clientCreationSettings =
                new SubscriberClient.ClientCreationSettings(null, null, credential.ToChannelCredentials());

            var subscriptionName = new SubscriptionName(gcpProjectId, pubSubTopicId);
            var subscriber = await SubscriberClient.CreateAsync(subscriptionName, clientCreationSettings);

            Console.WriteLine(@"Receiving transmissions ...");
            Console.WriteLine(string.Empty);

            var ordersMeta = new List<bool>();

            Console.Clear();
            Console.CursorSize = 72;
            Console.ForegroundColor = ConsoleColor.DarkGreen;

            await subscriber.StartAsync(async (msg, cancellationToken) =>
            {
                //receivedMessages.Add(msg);
                //Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
                //Console.WriteLine($"Data: '{msg.Data.ToStringUtf8()}'");

                var data = msg.Data.ToStringUtf8();
                var order = JsonConvert.DeserializeObject<Order>(data);

                ordersMeta.Add(order.Complete);

                var completeOrders = Convert.ToInt64(ordersMeta.Count(o => o));
                var orderConversion = completeOrders / (double) ordersMeta.Count;

                var orderConversionRate = orderConversion * 100;
                //Console.Clear();
                Console.WriteLine(orderConversionRate + "%");

                return await Task.FromResult(SubscriberClient.Reply.Ack);
            });

            Console.ReadLine();
            await subscriber.StopAsync(TimeSpan.Zero);
        }
    }
}