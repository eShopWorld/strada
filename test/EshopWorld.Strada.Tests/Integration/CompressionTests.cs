using System;
using System.Text;
using System.Threading;
using Eshopworld.Strada.Clients.Core;
using Microsoft.ServiceBus.Messaging;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration
{
    public class CompressionTests
    {
        /// <summary>
        ///     Ensures that compressed Event Hubs metadata is decompressed
        ///     by Azure Stream Analytics, and output to Azure Service Bus.
        /// </summary>
        [Fact]
        public void StreamAnalyticsDecompressesCompressedEventHubsTransmission()
        {
            var metadata = Encoding.UTF8
                .GetBytes(Resources.EventHubsPayload)
                .Compress();

            EventHubClient eventHubClient = null;
            ServicebusAdapter servicebusAdapter = null;

            try
            {
                eventHubClient = EventHubClient.CreateFromConnectionString(
                    Resources.EventHubsConnectionString,
                    Resources.EventHubsName);

                eventHubClient.Send(new EventData(metadata));

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