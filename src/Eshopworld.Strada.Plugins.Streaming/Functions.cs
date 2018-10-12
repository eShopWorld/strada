using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json.Linq;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     Functions provides functionality that is not specific to the data-transmission domain context.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        ///     AddTrackingMetadataToJson adds tracking metadata to a JSON payload.
        /// </summary>
        /// <param name="json">The JSON to add to the metadata.</param>
        /// <param name="brandCode">The customer reference code.</param>
        /// <param name="eventName">The name of the upstream event in which the JSON payload was raised.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <param name="queryString">The HTTP request Query string.</param>
        /// <param name="timestamp">The timestamp at which this method is called.</param>
        /// <param name="userAgent">The HTTP request User Agent header value.</param>
        /// <returns>The original JSON suffixed with <see cref="brandCode" /> and <see cref="correlationId" />.</returns>
        public static string AddTrackingMetadataToJson(
            string json,
            string brandCode,
            string eventName,
            string correlationId,
            string userAgent,
            string queryString,
            string timestamp)
        {
            var jsonObject = JObject.Parse(json);
            jsonObject.Add(new JProperty("brandCode", brandCode));
            jsonObject.Add(new JProperty("eventName", eventName));
            jsonObject.Add(new JProperty("correlationId", correlationId));
            jsonObject.Add(new JProperty("userAgent", userAgent));
            jsonObject.Add(new JProperty("queryString", queryString));
            jsonObject.Add(new JProperty("created", timestamp));
            return jsonObject.ToString();
        }

        /// <summary>
        ///     GetCorrelationId returns the correlation-id (an id that groups common data structures) from the current HTTP
        ///     context.
        /// </summary>
        /// <param name="httpRequest">The current HTTP context</param>
        /// <param name="correlationIdHeaderName">The HTTP header in which the correlation-id is present.</param>
        /// <remarks>Compatible with ASP.NET Core only.</remarks>
        public static string GetCorrelationId(HttpRequest httpRequest, string correlationIdHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var gotCorrelationId = httpRequest.Headers.TryGetValue(correlationIdHeaderName, out var headerValues);
            if (!gotCorrelationId) throw new Exception("Unable to read correlation-id from HTTP header.");

            string correlationId;
            if (!StringValues.IsNullOrEmpty(headerValues))
                correlationId = headerValues.LastOrDefault();
            else
                throw new Exception("Unable to read correlation-id from HTTP header.");

            return correlationId;
        }
    }
}