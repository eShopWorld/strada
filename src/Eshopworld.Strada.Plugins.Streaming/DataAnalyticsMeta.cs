namespace Eshopworld.Strada.Plugins.Streaming
{
    /// <summary>
    ///     DataAnalyticsMeta provides metadata necessary for downstream analysis.
    /// </summary>
    public class DataAnalyticsMeta
    {
        /// <summary>
        ///     Used to link related metadata in the downstream data lake.
        /// </summary>
        public string Fingerprint { get; set; }
    }
}