using System.Net.Http.Headers;
using System.Web.Http;
using Newtonsoft.Json.Serialization;
using System.Net.Http.Extensions.Compression.Core.Compressors;
using Microsoft.AspNet.WebApi.Extensions.Compression.Server;

namespace MusicMashup
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MessageHandlers.Insert(0, new ServerCompressionHandler(new GZipCompressor(), new DeflateCompressor()));

            var jsonFormatter = config.Formatters.JsonFormatter;
            jsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

            var serializerSettings = jsonFormatter.SerializerSettings;
            serializerSettings.ContractResolver = new LowerCasePropertyNamesContractResolver();
        }

        class LowerCasePropertyNamesContractResolver : DefaultContractResolver
        {
            protected override string ResolvePropertyName(string propertyName)
            {
                return propertyName.ToLower();
            }
        }
    }
}
