namespace Eshopworld.Strada.Web
{
    /// <summary>
    ///     DataAnalyticsMeta privides metadata necessary for downstream analysis.
    /// </summary>
    public class DataAnalyticsMeta
    {
        /// <summary>
        ///     Used to link related metadata in the downstream data lake.
        /// </summary>
        public string CorrelationId { get; set; }
    }
}