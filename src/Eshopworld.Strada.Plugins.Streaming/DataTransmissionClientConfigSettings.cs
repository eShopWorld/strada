namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataTransmissionClientConfigSettings
    {
        public DataTransmissionClientConfigSettings()
        {
        }

        /// <summary>
        ///     ///
        ///     <param name="projectId">The Cloud Pub/Sub Project ID.</param>
        ///     <param name="topicId">The Cloud Pub/Sub Topic ID</param>
        ///     <param name="swallowExceptions">
        ///         If <c>true</c>, invokes an exception-handling event on error, otherwise, the exception is thrown.
        ///     </param>
        ///     <param name="batchMode">Indicates whether or not to activate Pub/Sub batch mode.</param>
        ///     <param name="elementCountThreshold">
        ///         The element count (in seconds) above which further processing of a batch will
        ///         occur.
        ///     </param>
        ///     <param name="delayThreshold">The batch lifetime (in seconds) above which further processing of a batch will occur.</param>
        /// </summary>
        public DataTransmissionClientConfigSettings(
            string projectId,
            string topicId,
            bool swallowExceptions = true,
            bool batchMode = true,
            long elementCountThreshold = 1000,
            int delayThreshold = 3)
        {
            ProjectId = projectId;
            TopicId = topicId;
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
    }
}