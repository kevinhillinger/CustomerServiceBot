using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderRoot : IDialog<object>
    {
        // Options for user to choose
        private const string OrderNumber = "Search by Number";
        private const string OrderAccount = "Search by Account";
        private const string OrderDate = "Search by Date";

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderNumber, OrderAccount, OrderDate },
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
                    case OrderNumber:
                        context.Call(new OrderNumber(), this.ResumeAfterOptionDialog);
                        break;

                    case OrderAccount:
                        context.Call(new OrderAccount(), this.ResumeAfterOptionDialog);
                        break;

                    case OrderDate:
                        context.Call(new OrderDate(), this.ResumeAfterOptionDialog);
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

        private async Task MessageReceivedAsync(IDialogContext context)
        {
            string message = $"MessageRecievedAsync_OrderRoot";
            await context.PostAsync(message);
        }
    }
}
