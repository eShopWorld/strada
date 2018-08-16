using Newtonsoft.Json.Linq;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class FunctionsTests
    {
        /// <summary>
        ///     Ensures that custom metadata is added to a JSON payload.
        /// </summary>
        [Fact]
        public void CustomMetadataIsAddedToJSON()
        {
            const string brandCode = "ESW";
            const string correlationId = "447348C4-ED5D-4C40-9167-FE848B198834";

            var updatedJSON = Functions.AddCustomJSONMetadata(
                Resources.SimpleJson,
                brandCode,
                correlationId);

            dynamic deserialised = JObject.Parse(updatedJSON);
            Assert.Equal(brandCode, deserialised["brandCode"].ToString());
            Assert.Equal(correlationId, deserialised["correlationId"].ToString());
        }
    }
}