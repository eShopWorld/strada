using System;
using System.IO;
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
        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            if (args == null || args.Length.Equals(0))
                throw new ArgumentNullException(nameof(args));
            var gcpProjectId = args[0];
            var pubSubTopicId = args[1];
            var jsonFilePath = args[2];
            var brandCode = args[3];
            var correlationId = args[4];

            var client = new HttpClient();
            var serviceCredentials = client.GetStringAsync(Resources.CredentialsFileUri).Result;
            Console.WriteLine(@"Service credentials downloaded ...");

            BootUp(serviceCredentials, gcpProjectId, pubSubTopicId);
            Console.WriteLine(@"Boot-up complete ...");

            try
            {
                if (!File.Exists(jsonFilePath))
                    throw new FileNotFoundException("File not found.");
                var json = await File.ReadAllTextAsync(jsonFilePath);
                Console.WriteLine(@"JSON extracted ...");

                DataTransmissionClient.Instance.Init(
                    gcpProjectId,
                    pubSubTopicId,
                    serviceCredentials);
                Console.WriteLine(@"Transmission client initialised ...");

                await DataTransmissionClient.Instance.TransmitAsync(brandCode, correlationId, json);
                Console.WriteLine(@"Data transmission complete ...");
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
                return;
            }

            await DataTransmissionClient.ShutDownAsync();
            Console.WriteLine(@"Shutdown complete. Press any key.");
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
    }
}