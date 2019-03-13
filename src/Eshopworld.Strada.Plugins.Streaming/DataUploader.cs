using System;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace Eshopworld.Strada.Plugins.Streaming
{
    public class DataUploader
    {
        public delegate void DataUploaderStartFailedEventHandler(object sender, DataUploaderStartFailedEventArgs e);

        private static readonly Lazy<DataUploader> InnerDataUploader =
            new Lazy<DataUploader>(() => new DataUploader());

        private IScheduler _scheduler;

        public static DataUploader Instance => InnerDataUploader.Value;

        public event DataUploaderStartFailedEventHandler DataUploaderStartFailed;

        public async Task StartAsync(
            DataTransmissionClient dataTransmissionClient,
            EventMetaCache eventMetaCache,
            int executionTimeInterval)
        {
            if (dataTransmissionClient == null) throw new ArgumentNullException(nameof(dataTransmissionClient));
            if (eventMetaCache == null) throw new ArgumentNullException(nameof(eventMetaCache));
            if (executionTimeInterval <= 0)
                throw new IndexOutOfRangeException("Execution time interval must be greater than 0.");

            try
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
            catch (Exception exception)
            {
                const string errorMessage = "Failed to start data-upload background task.";
                OnDataUploaderStartFailed(new DataUploaderStartFailedEventArgs(new Exception(errorMessage, exception)));
            }
        }

        protected virtual void OnDataUploaderStartFailed(DataUploaderStartFailedEventArgs e)
        {
            DataUploaderStartFailed?.Invoke(this, e);
        }
    }
}