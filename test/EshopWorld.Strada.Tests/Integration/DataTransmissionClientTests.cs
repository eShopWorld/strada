using Eshopworld.Strada.Clients.Azure;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration
{
    public class DataTransmissionClientTests
    {
        [Fact]
        public void DataIsTransmittedToEventHubs()
        {
            var dataTransmissionClient = new DataTransmissionClient();

            dataTransmissionClient.Init(Resources.EventHubsConnectionString, Resources.EventHubsName);
            dataTransmissionClient.Connect();

            dataTransmissionClient.Transmit(string.Empty, string.Empty);
        }
    }
}