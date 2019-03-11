using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Eshopworld.Strada.Plugins.Streaming.NetCore
{
    public class DataAnalyticsMiddleware
    {
        private readonly RequestDelegate _next;

        public DataAnalyticsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, DataAnalyticsMeta dataAnalyticsMeta)
        {
            dataAnalyticsMeta.Fingerprint = Functions.GetFingerprint(context.Request);
            await _next(context);
        }
    }
}