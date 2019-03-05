using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class HttpRequestMeta // todo: Add AdditionalMeta dictionary to hold brand-code, etc.
    {
        public Uri Uri { get; set; }
        public object Body { get; set; }
        public List<KeyValuePair<string, IEnumerable<string>>> HttpRequestHeaders { get; set; }
        public string Fingerprint { get; private set; }

        public static async Task<HttpRequestMeta> Create(HttpRequestMessage httpRequestMessage,
            bool parseAllHttpHeaders = true)
        {
            List<KeyValuePair<string, IEnumerable<string>>> httpRequestHeaders;
            if (!parseAllHttpHeaders)
                httpRequestHeaders = httpRequestMessage.Headers
                    .Where(header =>
                        UriMetaCache.Instance.AllowedHttpHeaders.Contains(header.Key.ToLowerInvariant()))
                    .ToList();
            else
                httpRequestHeaders =
                    new List<KeyValuePair<string, IEnumerable<string>>>(httpRequestMessage.Headers);

            return new HttpRequestMeta
            {
                Uri = httpRequestMessage.RequestUri,
                Body = JsonConvert.DeserializeObject(await httpRequestMessage.Content.ReadAsStringAsync()),
                HttpRequestHeaders = httpRequestHeaders,
                Fingerprint = Functions.GetFingerprint(httpRequestMessage)
            };
        }
    }
}