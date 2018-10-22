using System;
using Eshopworld.Strada.Plugins.Streaming;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Eshopworld.Strada.Web
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
            services.AddScoped<OrderRepository>();
            services.AddScoped<DomainServiceLayer>();
            // We must retain a reference to the same instance, to retain a single GCP connection
            services.AddSingleton<DataTransmissionClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            // Return a Singleton instance
            var dataTransmissionClient = app.ApplicationServices.GetService<DataTransmissionClient>();
            // Subscribe to error notifications
            dataTransmissionClient.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
            dataTransmissionClient.TransmissionFailed += DataTransmissionClient_TransmissionFailed;

            // Read GCP service credentials from app.config
            var gcpServiceCredentials = new GcpServiceCredentials();
            Configuration.GetSection("gcpServiceCredentials").Bind(gcpServiceCredentials);
            // Establish a persistent connection to GCP Pub/Sub
            dataTransmissionClient.InitAsync("eshop-bigdata", "synthetic-data-load", gcpServiceCredentials).Wait();
            // Configure custom middleware to handle correlation-id meta
            app.UseMiddleware<DataAnalyticsMiddleware>();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        /// <summary>
        ///     Invoked if an error occurs while calling DataTransmissionClient.Init.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Start-up error.", e.Exception);
        }

        /// <summary>
        ///     Invoked if an error occurs while calling DataTransmissionClient.TransmitAsync.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void DataTransmissionClient_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Start-up error.", e.Exception);
        }
    }
}