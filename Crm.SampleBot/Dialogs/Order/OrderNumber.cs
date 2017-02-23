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
            // load the LuisResult from context.UserData
            LuisResult result = new LuisResult();
            context.UserData.TryGetValue<LuisResult>("LuisResult", out result);

            // check if LuisResult contains an entity
            if ((result != null) && (result.Entities.Count > 0) && result.Entities.FirstOrDefault(item => item.Type == "builtin.number") != null)
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
            
            string message = $"Let me check on that for you..";
            await context.PostAsync(message);

            // call API and display results
            var attachment = await GetReceiptCard(orderNumber);

            if (attachment == null) {
                string error = $"Sorry.. I could not find that order for you.";
                await context.PostAsync(message);
            } else
            {
                var card = context.MakeMessage();
                card.Attachments.Add(attachment);
                await context.PostAsync(card);
            }

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(context);
        }

        private async Task<Attachment> GetReceiptCard(string orderNumber)
        {
            var order = await ordersApi.GetAsync(orderNumber);

            //check status of null
            if (order != null)
            {
                var receiptCard = new ReceiptCard
                {
                    Items = new List<ReceiptItem>
                {
                    new ReceiptItem("Account Number", order.AccountNumber),
                    new ReceiptItem("Date Ordered", order.OrderDate?.ToString()),
                    new ReceiptItem("Est. Ship Date", order.ShipmentDate?.ToString())
                },

                    Title = $"Order #{order.OrderNumber}",
                    Facts = new List<Fact> {
                    new Fact("Freight", order.Freight?.ToString()),
                    new Fact("Tax", order.Tax?.ToString())
                },

                    Total = order.Total.ToString()
                };

                return receiptCard.ToAttachment();
            }
            else
            {
                return null;
            }
        }
    }
}
