using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.PerfApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("App initiating ...");

            var gcpServiceCredentials = new GcpServiceCredentials
            {
                Type = "",
                ProjectId = "",
                PrivateKeyId = "",
                PrivateKey = "",
                ClientEmail = "",
                ClientId = "",
                AuthUri = "",
                TokenUri = "",
                AuthProviderX509CertUrl = "",
                ClientX509CertUrl = ""
            };

            var dataTransmissionClient = new DataTransmissionClient();
            dataTransmissionClient.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
            dataTransmissionClient.TransmissionFailed += DataTransmissionClient_TransmissionFailed;

            dataTransmissionClient.InitAsync(
                "",
                "",
                gcpServiceCredentials,
                true,
                true).Wait();

            Console.WriteLine("Connection established ...");

            var eventMetadataCache = new EventMetaCache();
            var stopwatch = new Stopwatch();
            var elapsedTime = new List<long>();

            var eventMetadataUploadRegistry = new Registry();
            eventMetadataUploadRegistry
                .Schedule(() => new EventMetadataUploadJob(
                    dataTransmissionClient,
                    eventMetadataCache,
                    stopwatch,
                    elapsedTime))
                .NonReentrant()
                .ToRunEvery(5)
                .Seconds();

            JobManager.Initialize(eventMetadataUploadRegistry);
            Console.WriteLine("Test sequence initializing ...press return key to shutdown");

            var random = new Random();
            while (true)
            {
                var numEventPayloadItems = random.Next(1, 10001);
                Parallel.For(0, numEventPayloadItems,
                    i => { eventMetadataCache.Add(new SimpleObject {Name = "TEST"}); });
                Thread.Sleep(10000);
            }

            Console.ReadLine();
        }

        private static void DataTransmissionClient_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
            Console.WriteLine("Transmission failed: " + e.Exception.Message);
        }

        private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
        {
            Console.WriteLine("Initialisation failed: " + e.Exception.Message);
        }
    }
}