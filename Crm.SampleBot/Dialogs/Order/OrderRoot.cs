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
        private const string OrderNumber = "Search by Order Number";
        private const string OrderDate = "Search by Order Date";
        private const string OrderAccount = "Search by Account Number";
        private readonly IDialogFactory dialogFactory;

        public OrderRoot(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        public async Task StartAsync(IDialogContext context)
        {
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderNumber, OrderDate, OrderAccount },
                String.Format("What would you like to do?"), "Not a valid option", 0);
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
                        context.Call(dialogFactory.Create<OrderNumber>(), this.ResumeAfterOptionDialog);
                        break;

                    case OrderAccount:
                        context.Call(dialogFactory.Create<OrderAccount>(), this.ResumeAfterOptionDialog);
                        break;

                    case OrderDate:
                        context.Call(dialogFactory.Create<OrderDate>(), this.ResumeAfterOptionDialog);
                        break;
                }
            }
            catch (TooManyAttemptsException ex)
            {
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
            //returning from order search.. return back to RootDialog
            context.Done(context);
        }
    }
}
