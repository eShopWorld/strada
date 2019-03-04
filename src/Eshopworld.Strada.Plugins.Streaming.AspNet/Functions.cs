using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public static class Functions
    {
        public static string GetCorGetFingerprint(
            HttpRequest httpRequest,
            string fingerprintHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            IEnumerable<string> headerValues = httpRequest.Headers.GetValues(fingerprintHeaderName);
            return headerValues?.LastOrDefault();
        }
    }
}