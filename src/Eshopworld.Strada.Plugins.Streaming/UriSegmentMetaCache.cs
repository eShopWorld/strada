using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class UriSegmentMetaCache
    {
        private static readonly Lazy<UriSegmentMetaCache> Lazy =
            new Lazy<UriSegmentMetaCache>(() => new UriSegmentMetaCache());

        public UriSegmentMetaCache()
        {
            UriSegmentMeta = new List<UriSegmentMeta>();
        }

        public static UriSegmentMetaCache Instance => Lazy.Value;

        public List<UriSegmentMeta> UriSegmentMeta { get; }

        public void Add(string uriSegmentName, IEnumerable<HttpMethod> allowedHttpMethods)
        {
            if (string.IsNullOrEmpty(uriSegmentName)) throw new ArgumentNullException(nameof(uriSegmentName));
            if (allowedHttpMethods == null) throw new ArgumentNullException(nameof(allowedHttpMethods));

            var uriSegmentMeta = new UriSegmentMeta
            {
                UriSegmentName = uriSegmentName.ToLowerInvariant(),
                AllowedHttpMethods = new HashSet<string>()
            };

            foreach (var allowedHttpMethod in allowedHttpMethods)
                uriSegmentMeta.AllowedHttpMethods.Add(allowedHttpMethod.Method);
            UriSegmentMeta.Add(uriSegmentMeta);
        }
    }
}