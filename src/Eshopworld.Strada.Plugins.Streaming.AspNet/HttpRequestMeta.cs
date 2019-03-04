using System;
using System.Collections.Generic;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class HttpRequestMeta // todo: Add AdditionalMeta dictionary to hold brandcode, etc.
    {
        public Uri Uri { get; set; }
        public object Body { get; set; }
        public List<KeyValuePair<string, IEnumerable<string>>> HttpRequestHeaders { get; set; }
        public string Fingerprint { get; set; }
    }
}