using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class DataUploader
    {
        public static void Start(DataTransmissionClient dataTransmissionClient)
        {
            var eventMetadataUploadRegistry = new Registry();
            eventMetadataUploadRegistry
                .Schedule(() => new EventMetadataUploadJob(dataTransmissionClient, EventMetaCache.Instance))
                .NonReentrant()
                .ToRunNow().AndEvery(5) // todo: Expose timespan during init: START HERE
                .Seconds();

            JobManager.Initialize(eventMetadataUploadRegistry);
        }
    }
}