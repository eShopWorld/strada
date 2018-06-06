using System;
using System.Text;
using System.Threading;
using Eshopworld.Strada.Clients.Core;
using Microsoft.ServiceBus.Messaging;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration
{
    public class AzureMetadataSizeLimitTests
    {
        [Fact]
        public void StreamAnalyticsDecompressesCompressedEventHubsTransmission()
        {
            // Compress the metadata

            var metadata = Encoding.UTF8
                .GetBytes(Resources.EventHubsPayload)
                .Compress();

            EventHubClient eventHubClient = null;            
            ServicebusAdapter servicebusAdapter = null;

            try
            {
                // Transmit metadata to Event Hubs         

                eventHubClient = EventHubClient.CreateFromConnectionString(
                    Resources.EventHubsConnectionString,
                    Resources.EventHubsName);

                eventHubClient.Send(new EventData(metadata));

                // Receive the Service Bus output

                var manualResetEvent = new ManualResetEvent(false);
                servicebusAdapter = new ServicebusAdapter(manualResetEvent);

                servicebusAdapter.Subscribe();
                manualResetEvent.WaitOne(TimeSpan.FromSeconds(30));

                Assert.Equal(10, servicebusAdapter.MessageCount);
            }
            finally
            {
                servicebusAdapter?.Unsubscribe().Wait();
                eventHubClient?.Close();
            }
        }
    }
}