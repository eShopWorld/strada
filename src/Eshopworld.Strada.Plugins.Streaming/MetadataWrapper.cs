namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     MetadataWrapper encapsulates metadata instances and provides supplemental information that describes the metadata.
    /// </summary>
    public class MetadataWrapper<T> where T : class
    {
        /// <summary>
        ///     BrandName is the customer name or reference code.
        /// </summary>
        public string BrandName { get; set; }

        /// <summary>
        ///     CorrelationId is used to link related metadata in the downstream data lake.
        /// </summary>
        public string CorrelationId { get; set; }

        /// <summary>
        ///     Metadata is the data model/metadata to transmit to Cloud Pub/Sub.
        /// </summary>
        public T Metadata { get; set; }
    }
}