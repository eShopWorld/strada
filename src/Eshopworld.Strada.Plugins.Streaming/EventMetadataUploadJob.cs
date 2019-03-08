using System.Threading.Tasks;
using Quartz;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class EventMetadataUploadJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var dataMap = context.JobDetail.JobDataMap;
            var eventMetaCache = (EventMetaCache) dataMap[nameof(EventMetaCache)];
            var dataTransmissionClient = (DataTransmissionClient) dataMap[nameof(DataTransmissionClient)];

            var eventMetadataPayloadBatch = eventMetaCache.GetEventMetadataPayloadBatch();
            await dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch);
        }
    }
}