using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Crm.SampleBot.Dialogs.Order;
using Crm.SampleBot.Dialogs.ServiceRepresentative;
using Crm.SampleBot.Dialogs.MoreOptions;
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

        [LuisIntent("Order")]
        public async Task Order(IDialogContext context, LuisResult result)
        {
            // store LuisResult in context userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderRoot>(context);
        }

        [LuisIntent("Order.Number")]
        public async Task OrderNumber(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderNumber>(context);
        }

        [LuisIntent("Order.Account")]
        public async Task OrderAccount(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderAccount>(context);
        }

        [LuisIntent("Order.Date")]
        public async Task OrderDate(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderDate>(context);
        }

        private async Task MessageReceivedAsync(IDialogContext context)
        {
            // clear the LUIS entities from userData
            context.UserData.RemoveValue("LuisResult");

            await context.PostAsync("How else can I help you? I can you find an order or find your customer service representative. What would you like to do?");

        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                var options = await result; //check option selected in the future to fork to another dialog
                StartDialogWithResumeAfter<OrderRoot>(context);
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

        private void StartDialogWithResumeAfter<TDialog>(IDialogContext context) where TDialog : IDialog<object>
        {
            context.Call(dialogFactory.Create<TDialog>(), this.ResumeAfterOptionDialog);
        }
    }
}
