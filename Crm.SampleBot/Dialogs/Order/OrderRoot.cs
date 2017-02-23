using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderRoot : IDialog<object>
    {
        //refer to the resource file ProjectName.Filename (minus the en-US)
        static ResourceManager rmOrderRoot = new ResourceManager("Crm.SampleBot.Resources.OrderRoot", Assembly.GetExecutingAssembly());
        static ResourceManager rmRootDialog = new ResourceManager("Crm.SampleBot.Resources.RootDialog", Assembly.GetExecutingAssembly());

        //Set the language to be used; you can change this on-demand to change the langauage across the app
        //You will pass this everytime you request a value from the resx file
        static CultureInfo ciEnglish = new CultureInfo("en-US");

        // Options for user to choose
        private static string OrderNumber = rmOrderRoot.GetString("OrderNumber", ciEnglish);
        private static string OrderDate = rmOrderRoot.GetString("OrderDate", ciEnglish);
        private static string OrderAccount = rmOrderRoot.GetString("OrderAccount", ciEnglish);
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
                String.Format(rmRootDialog.GetString("WhatToDo", ciEnglish)), rmRootDialog.GetString("NotValid", ciEnglish), 0);
        }

        private async Task OnOptionSelected(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                //capture which option then selected
                string optionSelected = await result;

                if (optionSelected == OrderNumber) {
                    context.Call(dialogFactory.Create<OrderNumber>(), this.ResumeAfterOptionDialog);
                }
                if (optionSelected == OrderAccount) {
                    context.Call(dialogFactory.Create<OrderAccount>(), this.ResumeAfterOptionDialog);
                }
                if (optionSelected == OrderDate) {
                    context.Call(dialogFactory.Create<OrderDate>(), this.ResumeAfterOptionDialog);
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
