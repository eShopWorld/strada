using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class BotTrackingHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            if (request.RequestUri.Segments.Contains("values"))
                try
                {
                    var requestBody = await request.Content.ReadAsStringAsync();
                    EventMetadataCache.Instance.Add(requestBody);
                }
                catch
                {
                    // ignored
                }

            var result = await base.SendAsync(request, cancellationToken);
            return result;
        }
    }
}