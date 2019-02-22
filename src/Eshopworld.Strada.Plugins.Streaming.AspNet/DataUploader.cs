using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class DataUploader
    {
        public static void Start(DataTransmissionClient dataTransmissionClient, int executionTimeInterval)
        {
            var eventMetadataUploadRegistry = new Registry();
            eventMetadataUploadRegistry
                .Schedule(() => new EventMetadataUploadJob(dataTransmissionClient, EventMetaCache.Instance))
                .NonReentrant()
                .ToRunNow().AndEvery(executionTimeInterval)
                .Seconds();

            JobManager.Initialize(eventMetadataUploadRegistry);
        }
    }
}