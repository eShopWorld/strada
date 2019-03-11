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
                // todo: Swallow exception
            }

            if (!uriSegmentFound) return await base.SendAsync(request, cancellationToken);

            if (!allowedHttpMethods.Contains(request.Method.Method))
                return await base.SendAsync(request, cancellationToken);

            try
            {
                var httpRequestMeta = await HttpRequestMeta.Create(
                    request,
                    Functions.GetFingerprint(request));

                EventMetaCache.Instance.Add(httpRequestMeta);
            }
            catch
            {
                // todo: Swallow exception
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}