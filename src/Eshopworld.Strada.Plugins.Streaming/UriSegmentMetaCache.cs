using System;
using System.Collections.Generic;

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
    }
}