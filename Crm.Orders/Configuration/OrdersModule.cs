using Autofac;
using Crm.Orders.Client;
using Microsoft.Bot.Builder.Internals.Fibers;

namespace Crm.Orders.Configuration
{
    public class OrdersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OrdersApi>()
                .UsingConstructor(typeof(ApiConfiguration))
                .Keyed<IOrdersApi>(FiberModule.Key_DoNotSerialize)
                .As<IOrdersApi>();
        }
    }
}
