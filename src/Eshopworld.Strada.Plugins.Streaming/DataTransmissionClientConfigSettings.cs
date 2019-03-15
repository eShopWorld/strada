﻿namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientConfigSettings // todo: MAX THREADS config
    {
        public string ProjectId { get; set; }
        public string TopicId { get; set; }
        public long ElementCountThreshold { get; set; } = 1000;
        public int DelayThreshold { get; set; } = 3;
    }
}