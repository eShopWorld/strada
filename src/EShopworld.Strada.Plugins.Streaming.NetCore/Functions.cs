using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace EShopworld.Strada.Plugins.Streaming.NetCore
{
    /// <summary>
    ///     Functions provides functionality that is not specific to the data-transmission domain context.
    /// </summary>
    public static class Functions
    {        
        /// <summary>
        ///     GetCorrelationId returns the correlation-id (an id that groups common data structures) from the current HTTP
        ///     context.
        /// </summary>
        /// <param name="httpRequest">The current HTTP context</param>
        /// <param name="correlationIdHeaderName">The HTTP header in which the correlation-id is present.</param>
        /// <remarks>Compatible with ASP.NET Core only.</remarks>
        /// <remarks>Does not throw exceptions when correlation-id is not found, to ensure early-stage middleware is not impeded.</remarks>
        public static string GetCorrelationId(HttpRequest httpRequest, string correlationIdHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var gotCorrelationId = httpRequest.Headers.TryGetValue(correlationIdHeaderName, out var headerValues);
            if (!gotCorrelationId) return null;

            string correlationId = null;
            if (!StringValues.IsNullOrEmpty(headerValues))
                correlationId = headerValues.LastOrDefault();

            return correlationId;
        }
    }
}