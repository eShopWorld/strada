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
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <returns>The original JSON suffixed with <see cref="brandCode" /> and <see cref="correlationId" />.</returns>
        public static string AddTrackingMetadataToJson(
            string json,
            string brandCode,
            string correlationId)
        {
            var jsonObject = JObject.Parse(json);
            jsonObject.Add(new JProperty("correlationId", correlationId));
            jsonObject.Add(new JProperty("brandCode", brandCode));
            return jsonObject.ToString();
        }
    }
}