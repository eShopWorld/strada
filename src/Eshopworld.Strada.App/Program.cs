using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Google.Api.Gax;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Iam.V1;
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
                PrivateKeyId = "c475eb5893123613f11fe7b9a98e1e440f3132e2", // todo: Enter appropriate value
                PrivateKey = "-----BEGIN PRIVATE KEY-----\nMIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCKntOs8jSD5xj1\nzAdFKsjoFea+CnmZSVl/acKTBZ1g5BPxPorkI4FZ+dFN61FAkJ1God3sxePHfwlF\nJTnfNWjeUxFYcdYOVmtQ6+JQdEbGzOY59nMJGMfcDnZbc1ooRrXAQHqR3aQ7lB5z\np8RSzCpDg2nRJEforx2hm6brnrmfdfZ2dc57ocQttSsrHm9N6hbXmZdCgEafEmLq\nf8FLZzN2qWYcSqK+B/cpWrrjv5lHYkvokCFMPVenEJmpti2nSiNsiketNwXS5hsG\ngBKHKPEKgQhTqz7FpFU8T0unPd0W7DVekpkOmgPx0ZT/3uTw4JRRcz0vXCsvdGu3\nw3mW4fibAgMBAAECggEAJ5NRvpoFqTTUuPQPjU06P5xlHJOUBNHepdQ1c1ESjeN/\n2AuxjpGeb+g1O+g3cdX3uP8kg2E59y5LIL0QiSvP7U1M8fHKGZzNlbRKkAqqXD6K\najj0vb+f67ELVG+Z6U7KvAEMnVFofoP6r61RjQvx8wV1M/sGNqWsMYPlQ3a06xPQ\ngLcZaxbZe2lpArdNIUgW8AlRrRwOcXOU/q+m1OqGFZTDGOJOHOfOVumZXKy3Gdk3\nSlzDoqsdXxuki42flseiXUfq4wHjkKEhQRyJwqG7HV4x8s7RTFUqpFJWegDiw1eg\nXQJ2K1c7CSILIbumRjA+REzF9P7D3vrsUOn7RgZdyQKBgQC953fQi1cUfbSulhT5\nGbft/eBCcpgOEP5lVeBs10Egi92KupNvCmC1CLhLKdncv94tb+ABC4sod1rWm7ni\nUndYXkSvXRstH3HsL7wiFNoDtYqEVawghX9Wea7+FwGj1H91k83ZlPsmE3heclbe\nK3gSS/XGJg8VyLgNOtzAaW5uaQKBgQC63fUTgYfwuNlQ0Wz849EtbOm/4gFjdk4U\nMYjBDE2aBSu38yNFQMrx4q67oYrH0Pbj0KEaXszZrTrcH99lRicwiZeVP8g9nQ2x\nBzd8n+58Klh7e1lNerRib1faFNL2k4w+IZkQsfANK20Dr4cVEHmjeUZZiK+QELnK\n9Ja1IKZWYwKBgGvqL0IJDLEORjC4BK8RmF3b2SapbBMCQS5gwKnZIJ9YG8sL/2Ao\na5A0plXAMJerSJxVaNvvLWMPgEVYNSeRaVSELU/h/uGDbv+imKxdYQ6eiVpuPOQ2\nOIhxmam2dS4eQVgVZ/LvIFEg21QAcbDKzu7Gz6GMWyIr77tE6dFFmIWRAoGABjul\nExN8/1CLHyD9K6pFreg2G8pkFXc6v32vfBD2/a4yeR5JDOROYKcZAPUwdd3IfmzV\nEtYaqyAWGIWPpAclA39zaO0JttRoQoFlHmkPQWEANo40ulrfbXEdUw7iworFYMUq\nH95vjWDnb/oI3XEBPayr5gyjHoEQUW67ICuIoNUCgYBEVdtHXfD8HuAT94f+nB1L\nM5Czf7y4/DuTpnoE9GbeBEIkCDn0BkLS6KcrE9c6FGYP0/fVuuFvVkdht/xBW8Mi\nLM3eO8U1YEoaYXXkXA8b98hUr3oxejPvu+aI4svGXS/QH4va6v9+jNaxXl3YPBHi\nvckcX/r/18qSnIX5c2uSIQ==\n-----END PRIVATE KEY-----\n", // todo: Enter appropriate value
                ClientEmail = "dataflow-runner@eshop-bigdata.iam.gserviceaccount.com", // todo: Enter appropriate value
                ClientId = "103689787570041306234", // todo: Enter appropriate value
                AuthUri = "https://accounts.google.com/o/oauth2/auth",
                TokenUri = "https://oauth2.googleapis.com/token",
                AuthProviderX509CertUrl = "https://www.googleapis.com/oauth2/v1/certs",
                ClientX509CertUrl = "https://www.googleapis.com/robot/v1/metadata/x509/dataflow-runner%40eshop-bigdata.iam.gserviceaccount.com" // todo: Enter appropriate value
            };

            //TopicName topicName = new TopicName(gcpProjectId, pubSubTopicId);            

            //SubscriberServiceApiClient subscriberService = await SubscriberServiceApiClient.Create()
            SubscriptionName subscriptionName = new SubscriptionName(gcpProjectId, pubSubTopicId);

            var jsonServiceCredentials = JsonConvert.SerializeObject(serviceCredentials);

            var subscriberCredential = GoogleCredential.FromJson(jsonServiceCredentials)
                .CreateScoped(SubscriberServiceApiClient.DefaultScopes);
            //var subscriberChannel = new Channel(
            //    SubscriberServiceApiClient.DefaultEndpoint.ToString(),
            //    subscriberCredential.ToChannelCredentials());            

            //var subscriptionName = new SubscriptionName(
            //    gcpProjectId,
            //    pubSubTopicId);

            //SubscriberServiceApiSettings s1 = new SubscriberServiceApiSettings();
            //s1.

            List<PubsubMessage> receivedMessages = new List<PubsubMessage>();
            //var subscriber = SubscriberServiceApiClient.Create(subscriberChannel);

            SubscriberClient.ClientCreationSettings settings =
                new SubscriberClient.ClientCreationSettings(null, null, subscriberCredential.ToChannelCredentials(), null);

            //SubscriberClient.Settings s = new SubscriberClient.Settings();            

            SubscriberClient subscriber = await SubscriberClient.CreateAsync(subscriptionName, settings);            
            // Start the subscriber listening for messages.
            await subscriber.StartAsync((msg, cancellationToken) =>
            {
                receivedMessages.Add(msg);
                Console.WriteLine($"Received message {msg.MessageId} published at {msg.PublishTime.ToDateTime()}");
                Console.WriteLine($"Text: '{msg.Data.ToStringUtf8()}'");
                // Stop this subscriber after one message is received.
                // This is non-blocking, and the returned Task may be awaited.
                subscriber.StopAsync(TimeSpan.FromSeconds(15));
                // Return Reply.Ack to indicate this message has been handled.
                return Task.FromResult(SubscriberClient.Reply.Ack);
            });


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

        //private static async Task PullMessages(SubscriberServiceApiClient subscriber, bool acknowledge)
        //{
        //    await subscriber.(
        //        async (message, cancel) =>
        //        {
        //            var text =
        //                Encoding.UTF8.GetString(message.Data.ToArray());
        //            await Console.Out.WriteLineAsync(
        //                $"Message {message.MessageId}: {text}");
        //            return acknowledge
        //                ? SubscriberClient.Reply.Ack
        //                : SubscriberClient.Reply.Nack;
        //        });
        //    Thread.Sleep(3000);
        //    await subscriber.StopAsync(CancellationToken.None);
        //}

        private static TestIamPermissionsResponse TestSubscriptionIamPermissionsResponse(
            string projectId,
            string subscriptionId,
            SubscriberServiceApiClient subscriber)
        {
            List<string> permissions = new List<string>
            {
                "pubsub.subscriptions.get",
                "pubsub.subscriptions.update"
            };
            TestIamPermissionsRequest request = new TestIamPermissionsRequest
            {
                Resource = new SubscriptionName(projectId, subscriptionId).ToString(),
                Permissions = { permissions }
            };
            TestIamPermissionsResponse response = subscriber.TestIamPermissions(request);
            return response;
        }

    }
}