using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class DataTransmissionHandler : DelegatingHandler // todo: Need a remote kill-switch
    {
        protected override async Task<HttpResponseMessage> SendAsync( // todo: Consider caching results. 
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var uriSegmentFound = Streaming.Functions.UriSegmentExists(
                request.RequestUri.Segments,
                UriMetaCache.Instance.UriSegmentMeta,
                out var allowedHttpMethods);

            if (!uriSegmentFound) return await base.SendAsync(request, cancellationToken);

            if (!allowedHttpMethods.Contains(request.Method.Method))
                return await base.SendAsync(request, cancellationToken);

            var requestBody = JsonConvert.DeserializeObject(await request.Content.ReadAsStringAsync());

            var httpRequestMeta = new HttpRequestMeta
            {
                Uri = request.RequestUri,
                Body = requestBody,
                HttpRequestHeaders = request.Headers
                    .Where(header => UriMetaCache.Instance.AllowedHttpHeaders.Contains(header.Key.ToLowerInvariant()))
                    .ToList()
            };

            EventMetaCache.Instance.Add(httpRequestMeta); // todo: Add other parameters from HTTP headers

            return await base.SendAsync(request, cancellationToken);
        }
    }
}