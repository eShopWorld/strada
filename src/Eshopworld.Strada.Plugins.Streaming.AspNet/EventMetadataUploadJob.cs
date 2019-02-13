using System.Web.Hosting;
using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public class EventMetadataUploadJob : IJob, IRegisteredObject
    {
        private readonly DataTransmissionClient _dataTransmissionClient;
        private readonly EventMetadataCache _eventMetadataCache;

        public EventMetadataUploadJob( // todo: Move to ASP.NET Web API streaming project
            DataTransmissionClient dataTransmissionClient,
            EventMetadataCache eventMetadataCache)
        {
            _dataTransmissionClient = dataTransmissionClient;
            _eventMetadataCache = eventMetadataCache;
            HostingEnvironment.RegisterObject(this);
        }

        public void Execute()
        {
            try
            {
                var eventMetadataPayloadBatch = _eventMetadataCache.GetEventMetadataPayloadBatch();
                _dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch).Wait(); // todo: Exception handling
            }
            finally
            {
                HostingEnvironment.UnregisterObject(this);
            }
        }

        public void Stop(bool immediate)
        {
            HostingEnvironment.UnregisterObject(this);
        }
    }
}