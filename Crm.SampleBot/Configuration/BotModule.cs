using Autofac;
using Crm.SampleBot.Dialogs;
using Microsoft.Bot.Builder.Luis;

namespace Crm.SampleBot.Configuration
{
    /// <summary>
    /// DI module for registering types for the CRM bot library
    /// </summary>
    public class BotModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register<RootDialog>(c =>
            {
                var settings = c.Resolve<LuisModelSettings>();
                var service = new LuisService(new LuisModelAttribute(settings.ModelId, settings.SubscriptionKey));

                return new RootDialog(service);
            });

            builder.RegisterType<DialogFactory>().AsSelf();

            base.Load(builder);
        }
    }
}
