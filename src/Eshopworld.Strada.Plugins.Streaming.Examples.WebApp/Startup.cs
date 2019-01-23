﻿using System;
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
            services.AddScoped<OrderRepository>();
            services.AddScoped<DomainServiceLayer>();
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
            dataTransmissionClient.InitAsync("eshop-puddle", "checkout-dev", gcpServiceCredentials).Wait();
            // Configure custom middleware to handle correlation-id meta
            app.UseMiddleware<DataAnalyticsMiddleware>();

            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
        }

        private static void DataTransmissionClient_InitialisationFailed(object sender, InitialisationFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Start-up error.", e.Exception);
        }

        private static void DataTransmissionClient_TransmissionFailed(object sender, TransmissionFailedEventArgs e)
        {
            Console.WriteLine(e.Exception.Message);
            throw new Exception("Transmission error.", e.Exception);
        }
    }
}