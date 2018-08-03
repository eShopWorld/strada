namespace Eshopworld.Strada.Plugins.Streaming
{
    public class MetadataWrapper<T> where T : class
    {
        public string BrandName { get; set; }
        public T Metadata { get; set; }
    }
}