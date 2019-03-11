﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Eshopworld.Strada.Plugins.Streaming.NetCore
{
    public static class Functions
    {
        public static string GetFingerprint(
            HttpRequest httpRequest,
            string fingerprintHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var gotFingerprint = httpRequest.Headers.TryGetValue(fingerprintHeaderName, out var headerValues);
            if (!gotFingerprint) return null;

            string fingerprint = null;
            if (!StringValues.IsNullOrEmpty(headerValues))
                fingerprint = headerValues.LastOrDefault();

            return fingerprint;
        }
    }
}