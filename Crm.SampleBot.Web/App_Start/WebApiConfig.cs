using Autofac;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Web.Http;

namespace Crm.SampleBot.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            SetDefaultJsonSerializionSettings();
            ApplyJsonFormatting(config);

            var builder = new ContainerBuilder();
            ContainerConfig.Configure(config, builder);
        }

        private static void SetDefaultJsonSerializionSettings()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Newtonsoft.Json.Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        private static void ApplyJsonFormatting(HttpConfiguration config)
        {
            config.Formatters.JsonFormatter.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Formatting.Indented;
        }
    }
}
