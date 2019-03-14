using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming.Tests.Unit
{
    internal class PayloadMetadata
    {
        public string BrandCode { get; set; }
        public string EventName { get; set; }

        [JsonProperty("correlationId")] public string Fingerprint { get; set; }

        public string QueryString { get; set; }
        public Dictionary<string, string> HttpHeaders { get; set; }
        public string Created { get; set; }
    }
}