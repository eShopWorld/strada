using System;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Integration.WebApi;
using Newtonsoft.Json;

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

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            var gcpServiceCredentials =
                JsonConvert.DeserializeObject<CloudServiceCredentials>(Resources.GcpServiceCredentials);

            var dataTransmissionClientConfigSettings =
                JsonConvert.DeserializeObject<DataTransmissionClientConfigSettings>(
                    Resources.DataTransmissionClientConfigSettings);

            DataTransmissionClient.Instance.InitAsync(
                gcpServiceCredentials,
                dataTransmissionClientConfigSettings
            ).Wait();

            AspNet.DataUploader.Instance.StartAsync(
                DataTransmissionClient.Instance,
                EventMetaCache.Instance).Wait();
        }

        private static void EventMetadataUploadJobListener_EventMetadataUploadJobExecutionFailed(
            object sender,
            EventMetadataUploadJobExecutionFailedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}