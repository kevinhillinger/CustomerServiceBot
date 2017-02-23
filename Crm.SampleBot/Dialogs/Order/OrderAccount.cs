using Crm.Orders;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderAccount : IDialog<object>
    {
        private readonly IOrdersApi ordersApi;

        public OrderAccount(IOrdersApi ordersApi)
        {
            this.ordersApi = ordersApi;
        }

        public async Task StartAsync(IDialogContext context)
        {
            string message = $"Inside OrderAccount";
            await context.PostAsync(message);

            //var orders = await ordersApi.GetAsync(new QueryOptions { AccountNumber = "" });
            //orders.ForEach(order =>
            //{
            //    //ca
            //});

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(context);
        }
    }
}
