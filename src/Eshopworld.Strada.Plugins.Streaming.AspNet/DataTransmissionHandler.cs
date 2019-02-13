using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    // todo: Unit test
    public class DataTransmissionHandler : DelegatingHandler // todo: Need a remote kill-switch
    {
        protected override async Task<HttpResponseMessage> SendAsync( // todo: Consider caching results. 
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uriSegmentFound = false;
            var counter = 0;
            string uriSegmentName = null;

            if (UriSegmentMetaCache.Instance.UriSegmentMeta == null)
                return await base.SendAsync(request, cancellationToken);

            var uriSegments = new List<string>(request.RequestUri.Segments);

            try
            {
                do
                {
                    uriSegmentName = uriSegments[counter];
                    if (UriSegmentMetaCache.Instance.UriSegmentMeta.Any(meta => meta.UriSegmentName == uriSegmentName))
                        uriSegmentFound = true;
                } while (!uriSegmentFound && ++counter < uriSegments.Count);
            }
            catch
            {
                // ignored
            }

            if (!uriSegmentFound) return await base.SendAsync(request, cancellationToken);

            var allowedHttpMethods = UriSegmentMetaCache.Instance.UriSegmentMeta
                .First(meta => meta.UriSegmentName == uriSegmentName).AllowedHttpMethods;

            if (!allowedHttpMethods.Contains(request.Method.Method))
                return await base.SendAsync(request, cancellationToken);

            try
            {
                var requestBody = await request.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(requestBody)) EventMetadataCache.Instance.Add(requestBody);
            }
            catch
            {
                // ignored
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}