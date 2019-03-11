namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientConfigSettings
    {
        public string ProjectId { get; set; }
        public string TopicId { get; set; }
        public bool SwallowExceptions { get; set; }
        public bool BatchMode { get; set; }
        public long ElementCountThreshold { get; set; }
        public int DelayThreshold { get; set; }
    }
}