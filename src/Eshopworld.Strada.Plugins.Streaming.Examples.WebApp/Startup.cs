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
            services.AddSingleton<DataTransmissionClient>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();
            else
                app.UseHsts();

            DataTransmissionClient.Instance.InitialisationFailed += DataTransmissionClient_InitialisationFailed;
            DataTransmissionClient.Instance.TransmissionFailed += DataTransmissionClient_TransmissionFailed;

            var gcpServiceCredentials = new GcpServiceCredentials();
            Configuration.GetSection("gcpServiceCredentials").Bind(gcpServiceCredentials);

            app.UseMiddleware<DataTransmissionMiddleware>();
            app.UseHttpsRedirection();
            app.UseMvc();
            app.UseDefaultFiles();
            app.UseStaticFiles();
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