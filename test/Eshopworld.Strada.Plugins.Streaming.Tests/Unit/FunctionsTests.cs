using Newtonsoft.Json.Linq;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class FunctionsTests
    {
        /// <summary>
        ///     Ensures that tracking metadata is added to a JSON payload.
        /// </summary>
        [Fact]
        public void TrackingMetadataIsAddedToJSON()
        {
            const string brandCode = "ESW";
            const string eventName = "BOOTUP";
            const string correlationId = "447348C4-ED5D-4C40-9167-FE848B198834";
            const string created = "1538645229";

            var updatedJSON = Functions.AddTrackingMetadataToJson(
                Resources.JSON,
                brandCode,
                eventName,
                correlationId,
                created);

            dynamic deserialised = JObject.Parse(updatedJSON);
            Assert.Equal(brandCode, deserialised["brandCode"].ToString());
            Assert.Equal(eventName, deserialised["eventName"].ToString());
            Assert.Equal(correlationId, deserialised["correlationId"].ToString());
            Assert.Equal(created, deserialised["created"].ToString());
        }
    }
}