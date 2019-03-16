using System;
using Eshopworld.Strada.Plugins.Streaming.NetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddScoped<DataAnalyticsMeta>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            var cloudServiceCredentials = new CloudServiceCredentials();
            Configuration.GetSection("cloudServiceCredentials").Bind(cloudServiceCredentials);

            var dataTransmissionClientConfigSettings = new DataTransmissionClientConfigSettings();
            Configuration.GetSection("dataTransmissionClientConfigSettings").Bind(dataTransmissionClientConfigSettings);

            dataTransmissionClientConfigSettings.MaxThreadCount = Environment.ProcessorCount;

            DataTransmissionClient.Instance.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
            DataTransmissionClient.Instance.TransmissionFailed += DataTransmissionClient_TransmissionFailed;
            DataTransmissionClient.Instance.DataTransmitted += DataTransmissionClient_DataTransmitted;
            EventMetaCache.Instance.EventMetaAdded += EventMetaCache_EventMetaAdded;
            EventMetaCache.Instance.AddEventMetaFailed += EventMetaCache_AddEventMetaFailed;
            EventMetaCache.Instance.GetEventMetadataPayloadBatchFailed +=
                EventMetaCache_GetEventMetadataPayloadBatchFailed;
            EventMetaCache.Instance.GotEventMetadataPayloadBatch += Instance_GotEventMetadataPayloadBatch;
            EventMetaCache.Instance.ClearCacheFailed += EventMetaCache_ClearCacheFailed;
            DataUploader.Instance.DataUploaderStartFailed += Instance_DataUploaderStartFailed;
            DataUploader.Instance.EventMetadataUploadJobExecutionFailed +=
                DataUploader_EventMetadataUploadJobExecutionFailed;
            DataUploader.Instance.EventMetadataUploadJobExecutionFailed +=
                DataUploader_EventMetadataUploadJobExecutionFailed_2;

            DataTransmissionClient
                .Instance
                .InitAsync(cloudServiceCredentials, dataTransmissionClientConfigSettings)
                .Wait();

            DataUploader
                .Instance
                .StartAsync(DataTransmissionClient.Instance, EventMetaCache.Instance, 30,
                    dataTransmissionClientConfigSettings.MaxThreadCount)
                .Wait();

            app.UseMiddleware<DataTransmissionMiddleware>();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        private static void DataUploader_EventMetadataUploadJobExecutionFailed_2(
            object sender,
            EventMetadataUploadJobExecutionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.InnerException.Message);
        }

        private static void DataUploader_EventMetadataUploadJobExecutionFailed(
            object sender,
            EventMetadataUploadJobExecutionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.InnerException.Message);
        }

        private static void EventMetaCache_ClearCacheFailed(object sender, ClearCacheFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void EventMetadataUploadJobListener_EventMetadataUploadJobExecutionFailed(
            object sender,
            EventMetadataUploadJobExecutionFailedEventArgs e)
        {
            if (e.Exception != null) throw new NotImplementedException();
        }

        private static void Instance_DataUploaderStartFailed(object sender, DataUploaderStartFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void Instance_GotEventMetadataPayloadBatch(
            object sender,
            GetEventMetadataPayloadBatchEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void EventMetaCache_GetEventMetadataPayloadBatchFailed(
            object sender,
            GetEventMetadataPayloadBatchFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void EventMetaCache_AddEventMetaFailed(object sender, AddEventMetaFailedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private static void EventMetaCache_EventMetaAdded(object sender, EventMetaAddedEventArgs e)
        {
            Console.WriteLine(e.EventMeta.ToString());
        }

        private static void DataTransmissionClient_DataTransmitted(object sender, DataTransmittedEventArgs e)
        {
            Console.WriteLine(e.NumItemsTransferred);
        }

        private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Initialisation error.", e.Exception);
        }

        private static void DataTransmissionClient_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Transmission error.", e.Exception);
        }
    }
}