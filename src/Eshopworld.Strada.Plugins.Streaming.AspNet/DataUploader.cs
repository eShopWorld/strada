using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;

namespace Eshopworld.Strada.Plugins.Streaming.AspNet
{
    public sealed class DataUploader
    {
        public delegate void DataUploaderStartFailedEventHandler(object sender, DataUploaderStartFailedEventArgs e);

        private static readonly Lazy<DataUploader> InnerDataUploader =
            new Lazy<DataUploader>(() => new DataUploader());

        private EventMetadataUploadJobListener _eventMetadataUploadJobListener;
        private JobDetailImpl _jobDetail;

        private IScheduler _scheduler;
        private ITrigger _trigger;

        public static DataUploader Instance => InnerDataUploader.Value;

        public event DataUploaderStartFailedEventHandler DataUploaderStartFailed;

        public event EventMetadataUploadJobExecutionFailedEventHandler EventMetadataUploadJobExecutionFailed;

        [Obsolete("Please use overloaded implementation.")]
        public async Task StartAsync(
            DataTransmissionClient dataTransmissionClient,
            EventMetaCache eventMetaCache,
            int executionTimeInterval = 30,
            int maxThreadCount = 4)
        {
            if (dataTransmissionClient == null) throw new ArgumentNullException(nameof(dataTransmissionClient));
            if (eventMetaCache == null) throw new ArgumentNullException(nameof(eventMetaCache));
            if (executionTimeInterval <= 0)
                throw new IndexOutOfRangeException("Execution time interval must be greater than 0.");
            if (maxThreadCount <= 0)
                throw new IndexOutOfRangeException("Max thread-count must be greater than 0.");

            try
            {
                var factory = new StdSchedulerFactory(new NameValueCollection
                    {["quartz.threadPool.threadCount"] = maxThreadCount.ToString()});
                _scheduler = await factory.GetScheduler();

                await _scheduler.Start();

                const string jobName = "eventMetadataUploadJob";

                _jobDetail = new JobDetailImpl(
                    jobName,
                    typeof(EventMetadataUploadJob))
                {
                    JobDataMap =
                    {
                        [nameof(DataTransmissionClient)] = dataTransmissionClient,
                        [nameof(EventMetaCache)] = eventMetaCache
                    }
                };

                _eventMetadataUploadJobListener = new EventMetadataUploadJobListener();

                if (EventMetadataUploadJobExecutionFailed != null)
                    _eventMetadataUploadJobListener.EventMetadataUploadJobExecutionFailed +=
                        EventMetadataUploadJobExecutionFailed;

                _scheduler.ListenerManager.AddJobListener(
                    _eventMetadataUploadJobListener,
                    KeyMatcher<JobKey>.KeyEquals(new JobKey(jobName)));

                _trigger = TriggerBuilder.Create()
                    .WithSimpleSchedule(s => s
                        .WithIntervalInSeconds(executionTimeInterval)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(_jobDetail, _trigger);
            }
            catch (Exception exception)
            {
                const string errorMessage = "Failed to start data-upload background task.";
                OnDataUploaderStartFailed(new DataUploaderStartFailedEventArgs(new Exception(errorMessage, exception)));
            }
        }

        public async Task StartAsync(
            DataTransmissionClient dataTransmissionClient,
            EventMetaCache eventMetaCache,
            DataTransmissionClientConfigSettings dataTransmissionClientConfigSettings)
        {
            if (dataTransmissionClient == null) throw new ArgumentNullException(nameof(dataTransmissionClient));
            if (eventMetaCache == null) throw new ArgumentNullException(nameof(eventMetaCache));
            if (dataTransmissionClientConfigSettings == null)
                throw new ArgumentNullException(nameof(dataTransmissionClientConfigSettings));

            try
            {
                eventMetaCache.MaxQueueLength = dataTransmissionClientConfigSettings.MaxQueueLength;

                var factory = new StdSchedulerFactory(new NameValueCollection
                {
                    ["quartz.threadPool.threadCount"] = dataTransmissionClientConfigSettings.MaxThreadCount.ToString()
                });
                _scheduler = await factory.GetScheduler();

                await _scheduler.Start();

                const string jobName = "eventMetadataUploadJob";

                _jobDetail = new JobDetailImpl(
                    jobName,
                    typeof(EventMetadataUploadJob))
                {
                    JobDataMap =
                    {
                        [nameof(DataTransmissionClient)] = dataTransmissionClient,
                        [nameof(EventMetaCache)] = eventMetaCache
                    }
                };

                _eventMetadataUploadJobListener = new EventMetadataUploadJobListener();

                if (EventMetadataUploadJobExecutionFailed != null)
                    _eventMetadataUploadJobListener.EventMetadataUploadJobExecutionFailed +=
                        EventMetadataUploadJobExecutionFailed;

                _scheduler.ListenerManager.AddJobListener(
                    _eventMetadataUploadJobListener,
                    KeyMatcher<JobKey>.KeyEquals(new JobKey(jobName)));

                _trigger = TriggerBuilder.Create()
                    .WithSimpleSchedule(s => s
                        .WithIntervalInSeconds(dataTransmissionClientConfigSettings.ExecutionTimeInterval)
                        .RepeatForever())
                    .Build();

                await _scheduler.ScheduleJob(_jobDetail, _trigger);
            }
            catch (Exception exception)
            {
                const string errorMessage = "Failed to start data-upload background task.";
                OnDataUploaderStartFailed(new DataUploaderStartFailedEventArgs(new Exception(errorMessage, exception)));
            }
        }

        private void OnDataUploaderStartFailed(DataUploaderStartFailedEventArgs e)
        {
            DataUploaderStartFailed?.Invoke(this, e);
        }
    }
}