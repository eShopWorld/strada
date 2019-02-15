using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class FunctionsTests
    {        
        [Fact]
        public void TrackingMetadataIsAddedToJSON()
        {
            const string brandCode = "ESW";
            const string eventName = "BOOTUP";
            const string correlationId = "447348C4-ED5D-4C40-9167-FE848B198834";
            const string userAgent = "USER-AGENT";
            const string queryString = "QUERY-STRING";
            const string created = "1538645229";

            var updatedJSON = Functions.AddTrackingMetadataToJson(
                Resources.JSON,
                brandCode,
                eventName,
                correlationId,
                userAgent,
                queryString,
                created);

            dynamic deserialised = JObject.Parse(updatedJSON);
            Assert.Equal(brandCode, deserialised["brandCode"].ToString());
            Assert.Equal(eventName, deserialised["eventName"].ToString());
            Assert.Equal(correlationId, deserialised["correlationId"].ToString());
            Assert.Equal(userAgent, deserialised["userAgent"].ToString());
            Assert.Equal(queryString, deserialised["queryString"].ToString());
            Assert.Equal(created, deserialised["created"].ToString());
        }

        [Fact]
        public void UriSegmentIsFound()
        {
            const string httpPostMethod = "POST";
            const string uriSegmentName = "segment2";

            var uriSegmentMeta = new List<UriSegmentMeta>
            {
                new UriSegmentMeta
                {
                    UriSegmentName = uriSegmentName,
                    AllowedHttpMethods = new HashSet<string> {httpPostMethod}
                }
            };

            string[] uriSegments = {"segment1", uriSegmentName, "segment3"};
            var uriSegmentFound = Functions.UriSegmentExists(uriSegments, uriSegmentMeta, out var allowedHttpMethods);

            Assert.True(uriSegmentFound);
            Assert.Equal(httpPostMethod, allowedHttpMethods.First());
        }

        [Fact]
        public void UriSegmentIsNotFound()
        {
            const string httpPostMethod = "POST";
            const string uriSegmentName = "segment2";

            var uriSegmentMeta = new List<UriSegmentMeta>
            {
                new UriSegmentMeta
                {
                    UriSegmentName = "NAME",
                    AllowedHttpMethods = new HashSet<string> {httpPostMethod}
                }
            };

            string[] uriSegments = {"segment1", uriSegmentName, "segment3"};
            var uriSegmentFound = Functions.UriSegmentExists(uriSegments, uriSegmentMeta, out var allowedHttpMethods);

            Assert.False(uriSegmentFound);
        }
    }
}