using Autofac;
using Crm.Orders.Client;

namespace Crm.Orders.Configuration
{
    public class OrdersModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<OrdersApi>()
                .UsingConstructor(typeof(ApiConfiguration))
                .As<IOrdersApi>();
        }
    }
}
