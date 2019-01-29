using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming
{
    internal class EventMetadataUploadJob : IJob
    {
        private readonly DataTransmissionClient _dataTransmissionClient;
        private readonly EventMetadataCache _eventMetadataCache;

        public EventMetadataUploadJob(
            DataTransmissionClient dataTransmissionClient,
            EventMetadataCache eventMetadataCache)
        {
            _dataTransmissionClient = dataTransmissionClient;
            _eventMetadataCache = eventMetadataCache;
        }

        public void Execute()
        {
            var eventMetadataPayloadBatch = _eventMetadataCache.GetEventMetadataPayloadBatch();
            _dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch).Wait();
        }
    }
}