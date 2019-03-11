﻿using System.Collections.Generic;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientConfigSettings // todo: remove implicit properties, keep everything else
    {
        public DataTransmissionClientConfigSettings()
        {
            SwallowExceptions = true;
            BatchMode = true;
            ElementCountThreshold = 1000;
            DelayThreshold = 3;
        }

        public DataTransmissionClientConfigSettings(
            string projectId,
            string topicId,
            List<UriSegmentMeta> uriSegmentMeta,
            List<string> allowedHttpHeaders,
            bool swallowExceptions = true,
            bool batchMode = true,
            long elementCountThreshold = 1000,
            int delayThreshold = 3)
        {
            ProjectId = projectId;
            TopicId = topicId;
            UriSegmentMeta = uriSegmentMeta;
            AllowedHttpHeaders = allowedHttpHeaders;
            SwallowExceptions = swallowExceptions;
            BatchMode = batchMode;
            ElementCountThreshold = elementCountThreshold;
            DelayThreshold = delayThreshold;
        }

        public string ProjectId { get; set; }
        public string TopicId { get; set; }
        public bool SwallowExceptions { get; set; }
        public bool BatchMode { get; set; }
        public long ElementCountThreshold { get; set; }
        public int DelayThreshold { get; set; }
        public List<UriSegmentMeta> UriSegmentMeta { get; set; }
        public List<string> AllowedHttpHeaders { get; set; }
    }
}