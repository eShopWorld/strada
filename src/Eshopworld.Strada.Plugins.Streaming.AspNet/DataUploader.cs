using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class DataUploader
    {
        public static void Start(DataTransmissionClient dataTransmissionClient)
        {
            var eventMetadataUploadRegistry = new Registry();
            eventMetadataUploadRegistry
                .Schedule(() => new EventMetadataUploadJob(dataTransmissionClient, EventMetadataCache.Instance))
                .NonReentrant()
                .ToRunNow().AndEvery(5) // todo: Expose timespan during init
                .Seconds();

            JobManager.Initialize(eventMetadataUploadRegistry); // todo: .NET Core equivalent
        }
    }
}