using System;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Web.Controllers
{
    /// <summary>
    ///     DataAnalytics provides funtionality necessary to integrate with down-stream data analytics systems.
    /// </summary>
    public class DataAnalytics
    {
        /// <summary>
        ///     GetCorrelationId returns the correlation-id (an id that groups common data structures) from the current HTTP
        ///     context.
        /// </summary>
        /// <param name="httpRequest">The current HTTP context</param>
        /// <param name="correlationIdHeaderName">The HTTP header in which the correlation-id is present.</param>
        public string GetCorrelationId(HttpRequest httpRequest, string correlationIdHeaderName = "correlationid")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var gotCorrelationId = httpRequest.Headers.TryGetValue(correlationIdHeaderName, out var headerValues);
            var correlationId =
                headerValues.LastOrDefault(); // Get the most recent header when multiple headers are present

            return gotCorrelationId ? correlationId : null;
        }
    }
}