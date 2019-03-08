using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataUploader
    {
        private IScheduler _scheduler;

        public async Task StartAsync(
            DataTransmissionClient dataTransmissionClient,
            EventMetaCache eventMetaCache,
            int executionTimeInterval)
        {
            var factory = new StdSchedulerFactory();
            _scheduler = await factory.GetScheduler();

            await _scheduler.Start();

            var jobDetail = new JobDetailImpl(
                "eventMetadataUploadJob",
                typeof(EventMetadataUploadJob))
            {
                JobDataMap =
                {
                    [nameof(DataTransmissionClient)] = dataTransmissionClient,
                    [nameof(EventMetaCache)] = eventMetaCache
                }
            };

            var trigger = TriggerBuilder.Create()
                .WithSimpleSchedule(s => s
                    .WithIntervalInSeconds(executionTimeInterval)
                    .RepeatForever())
                .Build();

            await _scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}