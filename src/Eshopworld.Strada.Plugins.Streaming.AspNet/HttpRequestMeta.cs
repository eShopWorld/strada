using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    internal class HttpRequestMeta
    {
        public Uri Uri { get; set; }
        public object Body { get; set; }
        public List<KeyValuePair<string, IEnumerable<string>>> HttpRequestHeaders { get; set; }
    }
}