using Autofac;
using Crm.Orders;
using Crm.SampleBot.Dialogs;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Internals.Fibers;
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
                var ordersApi = c.Resolve<IOrdersApi>();
                var service = new LuisService(new LuisModelAttribute(settings.ModelId, settings.SubscriptionKey));

                return new RootDialog(service, ordersApi);
            })
            .Keyed<RootDialog>(FiberModule.Key_DoNotSerialize)
            .AsSelf();

            builder.RegisterType<DialogFactory>().AsSelf();

            base.Load(builder);
        }
    }
}
