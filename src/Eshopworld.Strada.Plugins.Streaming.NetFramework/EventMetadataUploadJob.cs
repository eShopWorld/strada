using System;
using System.Threading.Tasks;
using Quartz;

namespace Eshopworld.Strada.Plugins.Streaming.NetFramework
{
    public class EventMetadataUploadJob : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                var dataMap = context.JobDetail.JobDataMap;
                var eventMetaCache = (EventMetaCache) dataMap[nameof(EventMetaCache)];
                var dataTransmissionClient = (DataTransmissionClient) dataMap[nameof(DataTransmissionClient)];

                var eventMetadataPayloadBatch = eventMetaCache.GetEventMetadataPayloadBatch();
                await dataTransmissionClient.TransmitAsync(eventMetadataPayloadBatch);
            }
            catch (Exception exception)
            {
                const string errorMessage = "Event meta-upload background task execution failed.";
                throw new Exception(errorMessage, exception);
            }
        }
    }
}