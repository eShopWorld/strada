using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     DataAnalyticsMiddleware applies data analytics metadata to the HTTP context.
    /// </summary>
    /// <remarks>Compatible with ASP.NET Core only.</remarks>
    public class DataAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;

        public DataAnalyticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataAnalyticsMeta dataAnalyticsMeta)
        {
            dataAnalyticsMeta.CorrelationId = Functions.GetCorrelationId(context.Request);
            await _next(context);
        }
    }
}