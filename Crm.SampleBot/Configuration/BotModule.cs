﻿using Autofac;
using Crm.Orders;
using Crm.SampleBot.Dialogs;
using Crm.SampleBot.Dialogs.Order;
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
            builder.RegisterType<DialogFactory>()
                .Keyed<IDialogFactory>(FiberModule.Key_DoNotSerialize)
                .As<IDialogFactory>();

            builder.Register<ILuisService>(c => {
                var settings = c.Resolve<LuisModelSettings>();
                var service = new LuisService(new LuisModelAttribute(settings.ModelId, settings.SubscriptionKey));

                return service;
            });

            builder.RegisterType<RootDialog>()
            .Keyed<RootDialog>(FiberModule.Key_DoNotSerialize)
            .AsSelf();

            builder.RegisterType<OrderRoot>().AsSelf();
            builder.RegisterType<OrderStatus>().AsSelf();
            

            base.Load(builder);
        }
    }
}
