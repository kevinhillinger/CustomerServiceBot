using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Crm.Orders;

namespace Crm.SampleBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        // Options for user to choose
        private const string OrderStatusOption = "Check Order Status";
        private const string OpenOrdersOption = "Find Open Orders";
        private const string RepOption = "Find Representative";
        private readonly IOrdersApi ordersApi;

        public RootDialog(ILuisService service, IOrdersApi ordersApi)
            : base(service)
        {
            this.ordersApi = ordersApi;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, OpenOrdersOption, RepOption },
                String.Format($"Sorry, I did not understand '{result.Query}'.. What would you like to do?"), "Not a valid option");
        }

        [LuisIntent("greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, OpenOrdersOption, RepOption },
                String.Format("Hello! I am a CRM bot, I can get order status by number, find all open orders by person or find a person's sales rep. What would you like to do?"), "Not a valid option");
        }

        [LuisIntent("getOrderStatus")]
        public async Task GetOrderStatus(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            // Start new dialog
            context.Call(new OrderStatus(), this.ResumeAfterOptionDialog);
        }

        [LuisIntent("getOpenOrders")]
        public async Task GetOpenOrders(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            // start new dialog
            context.Call(new OpenOrders(), this.ResumeAfterOptionDialog);
        }

        [LuisIntent("getRep")]
        public async Task GetRep(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);
            
            // Start new dialog
            context.Call(new Representative(), this.ResumeAfterOptionDialog);
        }

        private async Task MessageReceivedAsync(IDialogContext context)
        {
            // clear the LUIS entities from userData
            context.UserData.RemoveValue("LuisResult");

            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, OpenOrdersOption, RepOption },
                String.Format("What would you like to do?"), "Not a valid option", 3);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                //capture which option then selected
                string optionSelected = await result;
                switch (optionSelected)
                {
                    case OrderStatusOption:
                        context.Call(new OrderStatus(), this.ResumeAfterOptionDialog);
                        break;

                    case OpenOrdersOption:
                        context.Call(new OpenOrders(), this.ResumeAfterOptionDialog);
                        break;

                    case RepOption:
                        context.Call(new Representative(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
                //If too many attempts we send error to user and start all over. 
                await context.PostAsync($"Ooops! Too many attemps :( You can start again!");

                //This sets us in a waiting state, after running the prompt again. 
                await this.MessageReceivedAsync(context);
            }
        }

        private async Task ResumeAfterOptionDialog(IDialogContext context, IAwaitable<object> result)
        {
            try
            {
                var message = await result;
            }
            catch (Exception ex)
            {
                await context.PostAsync($"Failed with message: {ex.Message}");
            }
            finally
            {
                await this.MessageReceivedAsync(context);
            }
        }
    }
}
