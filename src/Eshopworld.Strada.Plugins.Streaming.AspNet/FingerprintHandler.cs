using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class FingerprintHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return await base.SendAsync(request, cancellationToken);
        }
    }
}