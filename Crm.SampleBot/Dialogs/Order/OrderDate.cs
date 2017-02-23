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

                if (orderDate != null && IsDate(orderDate))
                {
                    context.UserData.SetValue("orderDate", orderDate);
                    var orders = await GetOrders(orderDate);

                    if (orders.Count == 0)
                    {
                        await context.PostAsync("Sorry. I could not find that order for you.");
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
            await context.PostAsync("What date would you like to search? (YYYY-MM-DD)");
            context.Wait(RequestOrderDateAsync);
        }

        public async Task RequestOrderDateAsync(IDialogContext context, IAwaitable<IMessageActivity> messageActivity)
        {
            var result = await messageActivity;
            var orderDate = result.Text;

            //if (!IsDate(orderDate))
            //{
            //    //TODO: do something about bad input
            //    await context.PostAsync("Invalid date...");
            //    return;
            //}

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
                card.AttachmentLayout = AttachmentLayoutTypes.Carousel;

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
            var heroCard = new HeroCard
            {
                Title = $"Order #" + order.OrderNumber,
                Text = $"Account Number: {order.AccountNumber}, Order Date: {order.OrderDate}, Total: {order.Total}"
                //        //$" {order.OrderDate}"
                //        //$" {order.Subtotal}"
                //        //$" {order.Freight}"
                //        //$" {order.Tax}"
                //        //$" {order.Total}"
            };

            return heroCard.ToAttachment();

        }

        private static bool IsDate(string value)
        {
            var regex = new Regex(@"^(?:(?:31(\/|-|\.)(?:0?[13578]|1[02]))\1|(?:(?:29|30)(\/|-|\.)(?:0?[1,3-9]|1[0-2])\2))(?:(?:1[6-9]|[2-9]\d)?\d{2})$|^(?:29(\/|-|\.)0?2\3(?:(?:(?:1[6-9]|[2-9]\d)?(?:0[48]|[2468][048]|[13579][26])|(?:(?:16|[2468][048]|[3579][26])00))))$|^(?:0?[1-9]|1\d|2[0-8])(\/|-|\.)(?:(?:0?[1-9])|(?:1[0-2]))\4(?:(?:1[6-9]|[2-9]\d)?\d{2})$");
            return regex.IsMatch(value);
        }
    }
}
