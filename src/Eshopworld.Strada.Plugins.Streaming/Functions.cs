using System;
using System.Collections.Generic;
using System.Linq;
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
            string userAgent,
            string queryString,
            string timestamp)
        {
            try
            {
                var jsonObject = JObject.Parse(json);
                jsonObject.Add(new JProperty("brandCode", brandCode));
                jsonObject.Add(new JProperty("eventName", eventName));
                jsonObject.Add(new JProperty("correlationId", fingerprint));
                jsonObject.Add(new JProperty("userAgent", userAgent));
                jsonObject.Add(new JProperty("queryString", queryString));
                jsonObject.Add(new JProperty("created", timestamp));
                return jsonObject.ToString();
            }
            catch (Exception exception)
            {
                throw new JsonSerializationException($"Could not edit the JSON payload: {json}", exception);
            }
        }

        public static bool UriSegmentExists(
            string[] uriSegments,
            List<UriSegmentMeta> uriSegmentMeta,
            out HashSet<string> allowedHttpMethods)
        {
            if (uriSegments == null) throw new ArgumentNullException(nameof(uriSegments));
            if (uriSegmentMeta == null) throw new ArgumentNullException(nameof(uriSegmentMeta));

            var uriSegmentFound = false;
            var index = 0;
            string uriSegmentName;

            try
            {
                do
                {
                    uriSegmentName = uriSegments[index].Replace("/", string.Empty);
                    if (uriSegmentMeta.Any(meta => meta.UriSegmentName == uriSegmentName))
                        uriSegmentFound = true;
                } while (!uriSegmentFound && ++index < uriSegments.Length);


                if (uriSegmentFound)
                {
                    allowedHttpMethods = uriSegmentMeta.First(meta => meta.UriSegmentName == uriSegmentName)
                        .AllowedHttpMethods;
                    return true;
                }

                allowedHttpMethods = null;
                return false;
            }
            catch
            {
                allowedHttpMethods = null;
                return false;
            }
        }
    }
}