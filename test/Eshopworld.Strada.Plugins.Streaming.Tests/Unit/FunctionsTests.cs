using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    public class FunctionsTests
    {
        /// <summary>
        ///     Ensures that Fingerprint/Correlation headers are retrieved from HTTP requests.
        /// </summary>
        [Fact]
        public void CorrelationIdValueIsDeterminedFromHttpRequest()
        {
            var expectedCorrelationId = Guid.NewGuid().ToString();

            HttpContext httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add("FingerprintId", expectedCorrelationId);

            var actualCorrelationId = Functions.GetCorrelationId(httpContext.Request);
            Assert.Equal(expectedCorrelationId, actualCorrelationId);
        }

        /// <summary>
        ///     Ensures that tracking metadata is added to a JSON payload.
        /// </summary>
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
    }
}