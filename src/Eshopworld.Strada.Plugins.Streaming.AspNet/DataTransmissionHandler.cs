using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class DataTransmissionHandler : DelegatingHandler // todo: Need a remote kill-switch
    {
        protected override async Task<HttpResponseMessage> SendAsync( // todo: Consider caching results. 
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uriSegmentFound = Functions.UriSegmentExists(
                request.RequestUri.Segments,
                UriSegmentMetaCache.Instance.UriSegmentMeta,
                out var allowedHttpMethods);

            if (!uriSegmentFound) return await base.SendAsync(request, cancellationToken);

            if (!allowedHttpMethods.Contains(request.Method.Method))
                return await base.SendAsync(request, cancellationToken);

            var requestBody = await request.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(requestBody)) EventMetadataCache.Instance.Add(requestBody);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}