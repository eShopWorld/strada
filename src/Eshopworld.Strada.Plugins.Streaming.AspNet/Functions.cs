using System;
using System.Linq;
using System.Net.Http;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public static class Functions
    {
        public static string GetFingerprint(
            HttpRequestMessage httpRequest,
            string fingerprintHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var headerValues = httpRequest.Headers.GetValues(fingerprintHeaderName);
            return headerValues?.LastOrDefault();
        }
    }
}