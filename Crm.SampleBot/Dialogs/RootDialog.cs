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
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, ServiceRepresentative, MoreOptions },
                String.Format("Hello! I am Ecolab CRM bot. What would you like to do?"), "Not a valid option");
        }

        [LuisIntent("Order")]
        public async Task Order(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialog<OrderRoot>(context);
        }

        [LuisIntent("Order.Number")]
        public async Task OrderNumber(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            // start new dialog
            context.Call(new OrderNumber(), this.ResumeAfterOptionDialog);
        }

        [LuisIntent("Order.Account")]
        public async Task OrderAccount(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);
            
            // Start new dialog
            context.Call(new OrderAccount(), this.ResumeAfterOptionDialog);
        }

        [LuisIntent("Order.Date")]
        public async Task OrderDate(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            // Start new dialog
            context.Call(new OrderDate(), this.ResumeAfterOptionDialog);
        }

        private async Task MessageReceivedAsync(IDialogContext context)
        {
            // clear the LUIS entities from userData
            context.UserData.RemoveValue("LuisResult");

            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, ServiceRepresentative, MoreOptions },
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
                        context.Call(new OrderRoot(), this.ResumeAfterOptionDialog);
                        break;

                    case ServiceRepresentative:
                        context.Call(new OrderRoot(), this.ResumeAfterOptionDialog);
                        break;

                    case MoreOptions:
                        context.Call(new OrderRoot(), this.ResumeAfterOptionDialog);
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

        private void StartDialog<TDialog>(IDialogContext context)
        {
            context.Call(dialogFactory.Create<OrderRoot>(), this.ResumeAfterOptionDialog);
        }
    }
}
