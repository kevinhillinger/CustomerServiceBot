using Crm.Orders;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderDate : IDialog<object>
    {
        const string DateIntentType = "builtin.date";

        private readonly IOrdersApi ordersApi;
     
        public OrderDate(IOrdersApi ordersApi)
        {
            this.ordersApi = ordersApi;
        }

        public async Task StartAsync(IDialogContext context)
        {
            var result = new LuisResult();

            if (context.UserData.TryGetValue("LuisResult", out result))
            {
                var orderDate = GetValue(result);
                context.UserData.SetValue("orderDate", orderDate);

                if (orderDate != null && IsDate(orderDate))
                {
                    var orders = await GetOrders(orderDate);

                    if (orders.Count == 0)
                    {
                        await context.PostAsync("Sorry..I could not find any orders for that date");
                    }
                    else
                    {
                        var card = context.MakeMessage();
                        card.Attachments = orders.Select(o => ToAttachment(o)).ToList();

                        await context.PostAsync(card);
                    }

                    return;
                }
            }

            // no value, so get one
            await context.PostAsync("What order number would you like to search?");
            context.Wait(RequestOrderDateAsync);

            context.Done(context);
        }

        public async Task RequestOrderDateAsync(IDialogContext context, IAwaitable<IMessageActivity> messageActivity)
        {
            var result = await messageActivity;
            var orderDate = result.Text;

            if (!IsDate(orderDate))
            {
                //TODO: do something about bad input
                await context.PostAsync("Invalid date...");
                return;
            }

            context.UserData.SetValue("orderDate", orderDate);

            await context.PostAsync("Let me check on that for you..");
            var orders = await GetOrders(orderDate);

            if (orders.Count == 0)
            {
                await context.PostAsync("Sorry..I could not find any orders for that date");
            }
            else
            {
                var card = context.MakeMessage();
                card.Attachments = orders.Select(o => ToAttachment(o)).ToList();

                await context.PostAsync(card);
            }

            context.Done(context);
        }

        private string GetValue(LuisResult result)
        {
            if (result == null || result.Entities.Count == 0)
            {
                return null;
            }

            var recommendation = result.Entities.FirstOrDefault(item => item.Type == DateIntentType);
            return recommendation == null ? null : recommendation.Entity;
        }

        private async Task<List<Orders.Model.Order>> GetOrders(string orderDate)
        {
            var orders = await ordersApi.GetAsync(new QueryOptions { OrderDate = orderDate });
            return orders;
        }

        private static Attachment ToAttachment(Orders.Model.Order order)
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

        private static bool IsDate(string value)
        {
            var regex = new Regex(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$");
            return regex.IsMatch(value);
        }
    }
}
