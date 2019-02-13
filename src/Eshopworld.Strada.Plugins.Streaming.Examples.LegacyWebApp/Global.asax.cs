﻿using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using FluentScheduler;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp
{
    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var builder = new ContainerBuilder();

            // Get your HttpConfiguration.
            var config = GlobalConfiguration.Configuration;

            // Register your Web API controllers.
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            // OPTIONAL: Register the Autofac filter provider.
            builder.RegisterWebApiFilterProvider(config);

            // OPTIONAL: Register the Autofac model binder provider.
            builder.RegisterWebApiModelBinderProvider();

            builder.RegisterType<DataTransmissionClient>()
                .AsSelf()
                .SingleInstance();

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var dataTransmissionClient = container.Resolve<DataTransmissionClient>();

            var gcpServiceCredentials = new GcpServiceCredentials
            {
                Type = "",
                ProjectId = "",
                PrivateKeyId = "",
                PrivateKey = "",
                ClientEmail = "",
                ClientId = "",
                AuthUri = "",
                TokenUri = "",
                AuthProviderX509CertUrl = "",
                ClientX509CertUrl = ""
            }; // todo: Parse from JSON

            var dataTransmissionClientConfigSettings = new DataTransmissionClientConfigSettings
            {
                ProjectId = "",
                TopicId = ""
            }; // todo: Parse from JSON

            dataTransmissionClient.InitAsync( // todo: This should be the only exposed component
                gcpServiceCredentials,
                dataTransmissionClientConfigSettings
            ).Wait(); // todo: Create synchronous equivalent            

            var eventMetadataUploadRegistry = new Registry(); // todo: Encapsulate FluentScheduler in custom component
            eventMetadataUploadRegistry
                .Schedule(() => new EventMetadataUploadJob(dataTransmissionClient, EventMetadataCache.Instance))
                .NonReentrant()
                .ToRunNow().AndEvery(5) // todo: Expose timespan during init
                .Seconds();

            JobManager.Initialize(eventMetadataUploadRegistry); // todo: .NET Core equivalent
        }
    }
}