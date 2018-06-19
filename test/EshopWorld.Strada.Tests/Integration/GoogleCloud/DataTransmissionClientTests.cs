using Eshopworld.Strada.Clients.GoogleCloud;
using Xunit;

namespace EshopWorld.Strada.Tests.Integration.GoogleCloud
{
    public class DataTransmissionClientTests
    {
        /// <summary>
        ///     Ensures that metadata is transmitted to a Cloud Pub/Sub Topic.
        /// </summary>
        [Fact]
        public void DataIsTransmittedToEventHubs()
        {
            var dataTransmissionClient = new DataTransmissionClient();

            dataTransmissionClient.Init(Resources.EventHubsConnectionString, Resources.EventHubsName);
            dataTransmissionClient.Init(Resources.ProjectId, Resources.PubSubTopicId);

            dataTransmissionClient.Transmit(string.Empty, string.Empty).Wait();
        }
    }
}