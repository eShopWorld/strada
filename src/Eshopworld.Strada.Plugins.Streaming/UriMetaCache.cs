using System;
using System.Collections.Generic;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class UriMetaCache
    {
        private static readonly Lazy<UriMetaCache> Lazy =
            new Lazy<UriMetaCache>(() => new UriMetaCache());

        public UriMetaCache()
        {
            UriSegmentMeta = new List<UriSegmentMeta>();
            AllowedHttpHeaders = new List<string>();
        }

        public static UriMetaCache Instance => Lazy.Value;

        public List<UriSegmentMeta> UriSegmentMeta { get; }

        public List<string> AllowedHttpHeaders { get; }
    }
}