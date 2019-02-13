using System.Collections.Generic;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class UriSegmentMeta
    {
        public string UriSegmentName { get; set; }
        public HashSet<string> AllowedHttpMethods { get; set; }
    }
}