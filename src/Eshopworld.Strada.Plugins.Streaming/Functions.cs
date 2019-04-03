using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     Functions provides functionality that is not specific to the data-transmission domain context.
    /// </summary>
    public static class Functions
    {
        public static string AddTrackingMetadataToJson(
            string json,
            string brandCode = null,
            string eventName = null,
            string fingerprint = null,
            string queryString = null,
            Dictionary<string, string> httpHeaders = null,
            string timestamp = null)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                if (!string.IsNullOrEmpty(brandCode)) jsonObject.Add(new JProperty("brandCode", brandCode));
                if (!string.IsNullOrEmpty(eventName)) jsonObject.Add(new JProperty("eventName", eventName));
                if (!string.IsNullOrEmpty(fingerprint)) jsonObject.Add(new JProperty("correlationId", fingerprint));
                if (!string.IsNullOrEmpty(queryString)) jsonObject.Add(new JProperty("queryString", queryString));
                if (httpHeaders != null) jsonObject.Add(new JProperty("httpHeaders", JObject.FromObject(httpHeaders)));
                if (timestamp != null) jsonObject.Add(new JProperty("created", timestamp));
                return jsonObject.ToString();
            }
            catch (Exception exception)
            {
                throw new JsonSerializationException($"Could not edit the JSON payload: {json}", exception);
            }
        }
    }
}