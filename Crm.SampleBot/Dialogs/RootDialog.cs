using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Crm.SampleBot.Dialogs.Order;
using Crm.Orders;

namespace Crm.SampleBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        // Options for user to choose
        private const string OrderStatusOption = "Check Order Status";
        private const string ServiceRepresentative = "Service Representative";
        private const string MoreOptions = "More Options";
        private readonly IOrdersApi ordersApi;
        private readonly IDialogFactory dialogFactory;

        public RootDialog(ILuisService service, IOrdersApi ordersApi, IDialogFactory dialogFactory)
            : base(service)
        {
            this.ordersApi = ordersApi;
            this.dialogFactory = dialogFactory;
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, ServiceRepresentative, MoreOptions },
                String.Format($"Sorry, I did not understand '{result.Query}'.. What would you like to do?"), "Not a valid option");
        }

        [LuisIntent("Greeting")]
        public async Task Greeting(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            await context.PostAsync("Hi! I'd love to help you! I can help you find an order or find your customer service representative. What would you like to do?");
        }

        [LuisIntent(LuisIntents.Order)]
        public Task OrderReceived(IDialogContext context, LuisResult result)
        {
            return ForwardToNewDialog<OrderRoot>(context, result);
        }

        [LuisIntent(LuisIntents.OrderNumber)]
        public Task OrderNumberReceived(IDialogContext context, LuisResult result)
        {
            return ForwardToNewDialog<OrderNumber>(context, result);
        }

        [LuisIntent(LuisIntents.OrderAccount)]
        public Task OrderAccountReceived(IDialogContext context, LuisResult result)
        {
            return ForwardToNewDialog<OrderAccount>(context, result);
        }

        [LuisIntent(LuisIntents.OrderDate)]
        public Task OrderDate(IDialogContext context, LuisResult result)
        {
            return ForwardToNewDialog<OrderDate>(context, result);
        }

        private Task MessageReceivedAsync(IDialogContext context)
        {
            ClearLuisEntities(context);

            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, ServiceRepresentative, MoreOptions },
                string.Format("What would you like to do?"), "Not a valid option", 3);

            return Task.CompletedTask;
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var options = await result; //check option selected in the future to fork to another dialog
                StartNewDialog<OrderRoot>(context);
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync($"Ooops! Too many attemps :( You can start again!");
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

        private void ClearLuisEntities(IDialogContext context)
        {
            context.UserData.RemoveValue("LuisResult");
        }

        private Task ForwardToNewDialog<TDialog>(IDialogContext context, LuisResult result) where TDialog : IDialog<object>
        {
            context.UserData.SetValue("LuisResult", result);
            context.Call(dialogFactory.Create<TDialog>(), this.ResumeAfterOptionDialog);

            return Task.CompletedTask;
        }

        private void StartNewDialog<TDialog>(IDialogContext context) where TDialog : IDialog<object>
        {
            context.Call(dialogFactory.Create<TDialog>(), this.ResumeAfterOptionDialog);
        }
    }
}
