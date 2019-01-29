using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class EventMetadataUploadRegistry : Registry
    {
        public EventMetadataUploadRegistry(
            DataTransmissionClient dataTransmissionClient,
            EventMetadataCache eventMetadataCache)
        {
            Schedule(() => new EventMetadataUploadJob(dataTransmissionClient, eventMetadataCache))
                .NonReentrant()
                .ToRunNow().AndEvery(30)
                .Seconds();
        }
    }
}