namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientConfigSettings
    {
        public string ProjectId { get; set; }
        public string TopicId { get; set; }
        public long ElementCountThreshold { get; set; } = 1000;
        public long RequestByteThreshold { get; set; } = 5242880; // MAX bytes 10485760 / 2
        public int DelayThreshold { get; set; } = 3;
        public int MaxThreadCount { get; set; } = 45;
        public int ExecutionTimeInterval { get; set; } = 1;
        public int MaxQueueLength { get; set; } = 175000;
    }
}