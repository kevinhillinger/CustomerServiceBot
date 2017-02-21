using Autofac;
using Autofac.Integration.WebApi;
using Crm.SampleBot.Configuration;
using System.Configuration;
using System.Reflection;
using System.Web.Http;

namespace Crm.SampleBot.Web
{
    public static class ContainerConfig
    {
        public static void Configure(HttpConfiguration config, ContainerBuilder builder)
        {
            builder.RegisterModule<BotModule>();
            SetLuisSettings(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = builder.Build();
  
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
            config.MapHttpAttributeRoutes();
        }

        private static void SetLuisSettings(ContainerBuilder builder)
        {
            builder.Register<LuisModelSettings>(c => new LuisModelSettings
            {
                ModelId = ConfigurationManager.AppSettings["LUIS_ID"],
                SubscriptionKey = ConfigurationManager.AppSettings["LUIS_KEY"]
            });
        }
    }
}