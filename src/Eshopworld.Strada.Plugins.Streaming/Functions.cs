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
            string brandCode,
            string eventName,
            string fingerprint,
            string queryString,
            Dictionary<string, string> httpHeaders,
            string timestamp)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                jsonObject.Add(new JProperty("brandCode", brandCode));
                jsonObject.Add(new JProperty("eventName", eventName));
                jsonObject.Add(new JProperty("correlationId", fingerprint));
                jsonObject.Add(new JProperty("queryString", queryString));
                jsonObject.Add(new JProperty("httpHeaders", JObject.FromObject(httpHeaders)));
                jsonObject.Add(new JProperty("created", timestamp));
                return jsonObject.ToString();
            }
            catch (Exception exception)
            {
                throw new JsonSerializationException($"Could not edit the JSON payload: {json}", exception);
            }
        }
    }
}