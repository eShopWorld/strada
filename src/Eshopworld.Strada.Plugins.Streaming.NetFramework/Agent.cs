using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class Agent
    {
        private static readonly Lazy<Agent> Lazy =
            new Lazy<Agent>(() => new Agent());

        public static Agent Instance => Lazy.Value;

        public event EventMetaCache.EventMetaAddedEventHandler EventMetaAdded;
        public event EventMetaCache.AddEventMetaFailedEventHandler AddEventMetaFailed;
        public event EventMetaCache.GetEventMetadataPayloadBatchFailedEventHandler GetEventMetadataPayloadBatchFailed;
        public event EventMetaCache.GetEventMetadataPayloadBatchEventHandler GotEventMetadataPayloadBatch;
        public event EventMetaCache.ClearCacheFailedEventHandler ClearCacheFailed;

        public event DataTransmissionClient.InitialisationFailedEventHandler InitialisationFailed;
        public event DataTransmissionClient.TransmissionFailedEventHandler TransmissionFailed;
        public event DataTransmissionClient.DataTransmittedEventHandler DataTransmitted;

        public void Start(
            CloudServiceCredentials cloudServiceCredentials,
            DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            EventMetaCache.Instance.EventMetaAdded += EventMetaAdded;
            EventMetaCache.Instance.AddEventMetaFailed += AddEventMetaFailed;
            EventMetaCache.Instance.GetEventMetadataPayloadBatchFailed += GetEventMetadataPayloadBatchFailed;
            EventMetaCache.Instance.GotEventMetadataPayloadBatch += GotEventMetadataPayloadBatch;
            EventMetaCache.Instance.ClearCacheFailed += ClearCacheFailed;

            DataTransmissionClient.Instance.InitialisationFailed += InitialisationFailed;
            DataTransmissionClient.Instance.TransmissionFailed += TransmissionFailed;
            DataTransmissionClient.Instance.DataTransmitted += DataTransmitted;

            DataTransmissionClient.Instance.InitAsync(
                cloudServiceCredentials,
                dataTransmissionClientConfigSettings
            ).Wait();

            JobScheduler.Instance.StartAsync(
                DataTransmissionClient.Instance,
                EventMetaCache.Instance,
                dataTransmissionClientConfigSettings).Wait();
        }

        public void AddEvent<T>(T eventMetadataPayload,
            string brandCode,
            string eventName,
            string fingerprint,
            string queryString = null,
            Dictionary<string, string> httpHeaders = null)
        {
            EventMetaCache.Instance.Add(
                eventMetadataPayload,
                brandCode,
                eventName,
                fingerprint,
                queryString,
                httpHeaders);
        }

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
                jsonObject.Add(httpHeaders != null
                    ? new JProperty("httpHeaders", JObject.FromObject(httpHeaders))
                    : new JProperty("httpHeaders", null));
                jsonObject.Add(new JProperty("created", timestamp));
                return jsonObject.ToString();
            }
            catch (Exception exception)
            {
                throw new JsonSerializationException($"Could not edit the JSON payload: {json}", exception);
            }
        }

        public static string GetFingerprint(
            HttpRequestMessage httpRequest,
            string fingerprintHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));
            if (httpRequest.Headers == null) throw new ArgumentNullException(nameof(httpRequest.Headers));

            var gotHeaderValues = httpRequest.Headers.TryGetValues(fingerprintHeaderName, out var headerValues);
            return gotHeaderValues ? headerValues.LastOrDefault() : null;
        }

        public static Dictionary<string, string>
            ParseHttpHeaders(HttpRequestHeaders httpRequestHeaders)
        {
            if (httpRequestHeaders == null) return null;

            var parsedHttpHeaders = new Dictionary<string, string>();

            foreach (var httpRequestHeader in httpRequestHeaders)
                parsedHttpHeaders.Add(httpRequestHeader.Key, httpRequestHeader.Value.LastOrDefault());

            return parsedHttpHeaders;
        }

        public static Dictionary<string, string>
            ParseHttpHeaders(NameValueCollection httpRequestHeadersCollection)
        {
            return httpRequestHeadersCollection.Cast<string>()
                .ToDictionary(k => k, k => httpRequestHeadersCollection[k]);
        }
    }
}