using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public static class Functions
    {
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