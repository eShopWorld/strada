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
        /// <param name="timestamp">The timestamp at which this method is called.</param>
        /// <returns>The original JSON suffixed with <see cref="brandCode" /> and <see cref="correlationId" />.</returns>
        public static string AddTrackingMetadataToJson(
            string json,
            string brandCode,
            string eventName,
            string correlationId,
            string timestamp)
        {
            var jsonObject = JObject.Parse(json);
            jsonObject.Add(new JProperty("brandCode", brandCode));
            jsonObject.Add(new JProperty("eventName", eventName));
            jsonObject.Add(new JProperty("correlationId", correlationId));
            jsonObject.Add(new JProperty("created", timestamp));
            return jsonObject.ToString();
        }
    }
}