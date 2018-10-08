using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Web
{
    /// <summary>
    ///     DataAnalyticsMiddleware applies data analytics metadata to the HTTP context.
    /// </summary>
    public class DataAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;

        public DataAnalyticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataAnalyticsMeta dataAnalyticsMeta)
        {
            try
            {
                // Read the correlation-id header sent from the UI
                dataAnalyticsMeta.CorrelationId = GetCorrelationId(context.Request);
            }
            catch (Exception exception)
            {
                await context.Response.WriteAsync(exception.Message);
            }

            await _next(context);
        }

        /// <summary>
        ///     GetCorrelationId returns the correlation-id (an id that groups common data structures) from the current HTTP
        ///     context.
        /// </summary>
        /// <param name="httpRequest">The current HTTP context</param>
        /// <param name="correlationIdHeaderName">The HTTP header in which the correlation-id is present.</param>
        private string GetCorrelationId(HttpRequest httpRequest, string correlationIdHeaderName = "FingerprintId")
        {
            if (httpRequest == null) throw new ArgumentNullException(nameof(httpRequest));

            var gotCorrelationId = httpRequest.Headers.TryGetValue(correlationIdHeaderName, out var headerValues);
            var correlationId =
                headerValues.LastOrDefault(); // Get the most recent header when multiple headers are present

            return gotCorrelationId ? correlationId : null;
        }
    }
}