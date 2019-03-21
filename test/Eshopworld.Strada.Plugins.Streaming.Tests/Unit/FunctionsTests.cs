using System.Collections.Generic;
using System.Collections.Specialized;
using Newtonsoft.Json;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class FunctionsTests
    {
        [Fact]
        public void HttpHeadersAreExtractedInKvFormat()
        {
            const string key1 = "KEY1";
            const string key2 = "KEY2";
            const string value1 = "VAL1";
            const string value2 = "VAL2";

            var httpHeadersCollection = new NameValueCollection {{key1, value1}, {key2, value2}};

            var httpHeaders = AspNet.Functions.ParseHttpHeaders(httpHeadersCollection);

            Assert.True(httpHeaders.ContainsKey(key1));
            Assert.True(httpHeaders.ContainsKey(key2));
            Assert.True(httpHeaders.ContainsValue(value1));
            Assert.True(httpHeaders.ContainsValue(value2));
        }

        [Fact]
        public void TrackingMetadataIsAddedToJSON()
        {
            const string brandCode = "ESW";
            const string eventName = "BOOTUP";
            const string fingerprint = "447348C4-ED5D-4C40-9167-FE848B198834";
            const string queryString = "QUERY-STRING";
            var httpHeaders = new Dictionary<string, string>
                {{"User-Agent", "USERAGENT"}, {"Content-Type", "CONTENT"}};
            const string created = "1538645229";

            var updatedJSON = Functions.AddTrackingMetadataToJson(
                "{}",
                brandCode,
                eventName,
                fingerprint,
                queryString,
                httpHeaders,
                created);

            var payloadMetadata = JsonConvert.DeserializeObject<PayloadMetadata>(updatedJSON);

            Assert.Equal("ESW", payloadMetadata.BrandCode);
            Assert.Equal("BOOTUP", payloadMetadata.EventName);
            Assert.Equal("447348C4-ED5D-4C40-9167-FE848B198834", payloadMetadata.Fingerprint);
            Assert.Equal("QUERY-STRING", payloadMetadata.QueryString);
            Assert.Equal("USERAGENT", payloadMetadata.HttpHeaders["User-Agent"]);
            Assert.Equal("CONTENT", payloadMetadata.HttpHeaders["Content-Type"]);
            Assert.Equal("1538645229", payloadMetadata.Created);
        }
    }
}