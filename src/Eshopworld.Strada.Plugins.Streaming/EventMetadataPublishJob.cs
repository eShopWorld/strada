using System;
using System.Threading.Tasks;
using Quartz;

namespace Eshopworld.Strada.Plugins.Streaming
{
    internal class EventMetadataPublishJob : IJob
    {
        private readonly DataTransmissionClient _dataTransmissionClient;
        private readonly EventMetadataCache _eventMetadataCache;

        public EventMetadataPublishJob(DataTransmissionClient dataTransmissionClient,
            EventMetadataCache eventMetadataCache)
        {
            _dataTransmissionClient = dataTransmissionClient;
            _eventMetadataCache = eventMetadataCache;
        }

        public Task Execute(IJobExecutionContext context)
        {
            var eventMetadataPayloadBatch = _eventMetadataCache.GetEventMetadataPayloadBatch();
            if (!_dataTransmissionClient.Initialised)
                throw new NullReferenceException("Data transmission client is not initialised.");

            return _dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch);
        }
    }
}