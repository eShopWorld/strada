using System.Collections.Generic;
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
            var uriSegmentFound = false;
            HashSet<string> allowedHttpMethods = null;
            try
            {
                uriSegmentFound = Streaming.Functions.UriSegmentExists(
                    request.RequestUri.Segments,
                    UriMetaCache.Instance.UriSegmentMeta,
                    out allowedHttpMethods);
            }
            catch
            {
                // ignored
            }

            if (!uriSegmentFound) return await base.SendAsync(request, cancellationToken);

            if (!allowedHttpMethods.Contains(request.Method.Method))
                return await base.SendAsync(request, cancellationToken);

            try
            {
                var httpRequestMeta = await HttpRequestMeta.Create(request);
                EventMetaCache.Instance.Add(httpRequestMeta);
            }
            catch
            {
                // ignored
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}