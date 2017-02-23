using Autofac;
using Autofac.Integration.WebApi;
using Crm.Orders.Client;
using Crm.Orders.Configuration;
using Crm.SampleBot.Configuration;
using Microsoft.Bot.Builder.Dialogs;
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
            builder.RegisterModule<OrdersModule>();

            SetLuisSettings(builder);
            ConfigureApi(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            var container = Conversation.Container;

            // need the below to fix a bug with serialization for injected dependencies into dialogs
            builder.Update(Conversation.Container);

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

        private static void ConfigureApi(ContainerBuilder builder)
        {
            var config = new ApiConfiguration(new ApiClient(ConfigurationManager.AppSettings["api:basePath"]));
            builder.Register<ApiConfiguration>(c => config);
        }
        
    }
}