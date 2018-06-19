using Eshopworld.Strada.Clients.Azure;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration.Azure
{
    public class DataTransmissionClientTests
    {
        /// <summary>
        ///     Ensures that metadata is transmitted to an Event Hub.
        /// </summary>
        [Fact]
        public void DataIsTransmittedToEventHubs()
        {
            var dataTransmissionClient = new DataTransmissionClient();

            dataTransmissionClient.Init(Resources.EventHubsConnectionString, Resources.EventHubsName);
            dataTransmissionClient.Connect();

            dataTransmissionClient.Transmit(string.Empty, string.Empty).Wait();
        }
    }
}