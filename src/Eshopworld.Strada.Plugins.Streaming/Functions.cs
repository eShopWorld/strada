using Newtonsoft.Json.Linq;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     Functions provides functionality that is not specific to the domain context.
    /// </summary>
    public static class Functions
    {
        /// <summary>
        ///     AddCustomJSONMetadata adds domain and tracking metadata to a JSON payload.
        /// </summary>
        /// <param name="json">The JSON to add to the metadata.</param>
        /// <param name="brandCode">The customer reference code.</param>
        /// <param name="correlationId">Used to link related metadata in the downstream data lake.</param>
        /// <returns></returns>
        public static string AddCustomJSONMetadata(string json, string brandCode, string correlationId)
        {
            var jsonObject = JObject.Parse(json);
            jsonObject.Add(new JProperty("correlationId", correlationId));
            jsonObject.Add(new JProperty("brandCode", brandCode));
            return jsonObject.ToString();
        }
    }
}