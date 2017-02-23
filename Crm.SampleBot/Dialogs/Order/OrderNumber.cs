using Crm.Orders;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderNumber : IDialog<object>
    {
        private readonly IOrdersApi ordersApi;

        public OrderNumber(IOrdersApi ordersApi)
        {
            this.ordersApi = ordersApi;
        }

        public async Task StartAsync(IDialogContext context)
        {
            string message = $"Inside OrderNumber";
            await context.PostAsync(message);

            // load the LuisResult from context.UserData
            LuisResult result = new LuisResult();
            context.UserData.TryGetValue<LuisResult>("LuisResult", out result);

            // check if LuisResult contains an entity
            if ((result != null) && (result.Entities.Count > 0))
            {
                // store orderNumber
                
                var orderNumber = result.Entities.First(item => item.Type == "builtin.number").Entity;
                context.UserData.SetValue<string>("orderNumber", orderNumber);

                // call API and display results
                await CallAPI(context);
            }
            else
            {
                // There is no entity, prompt user for the search paramater
                await context.PostAsync("What order number would you like to search?");

                // call function to store result
                context.Wait(getEntity);
            }
        }

        public async Task getEntity(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // we do not have an entity, take it from the argument
            //variable to hold message coming in
            var arg = await argument;
            var orderNumber = arg.Text;

            // store orderNumber
            context.UserData.SetValue<string>("orderNumber", orderNumber);

            // call API and display results
            await CallAPI(context);
        }

        public async Task CallAPI(IDialogContext context)
        {
            // retrieve orderNumber to be searched
            var orderNumber = "";
            context.UserData.TryGetValue<string>("orderNumber", out orderNumber);

            string message = $"getOrderStatus for order #{orderNumber}";
            await context.PostAsync(message);

            // Call API with orderNumber
            message = $"Calling API..";
            await context.PostAsync(message);

            // Display results
            message = $"Displaying API results..";
            await context.PostAsync(message);

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(context);

        }
    }
}
