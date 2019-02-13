using System.Web.Http;
using Eshopworld.Strada.Plugins.Streaming.AspNet;

namespace Eshopworld.Strada.Plugins.Streaming.Examples.LegacyWebApp
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                "DefaultApi",
                "api/{controller}/{id}",
                new {id = RouteParameter.Optional}
            );

            config.MessageHandlers.Add(new DataTransmissionHandler());
        }
    }
}