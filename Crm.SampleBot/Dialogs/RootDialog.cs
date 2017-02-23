using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Threading.Tasks;
using Crm.SampleBot.Dialogs.Order;
using System.Resources;
using System.Globalization;
using System.Reflection;

namespace Crm.SampleBot.Dialogs
{
    [Serializable]
    public class RootDialog : LuisDialog<object>
    {
        static ResourceManager rmRootDialog = new ResourceManager("Crm.SampleBot.Resources.RootDialog", Assembly.GetExecutingAssembly());

        //Set the language to be used; you can change this on-demand to change the langauage across the app
        //You will pass this everytime you request a value from the resx file
        static CultureInfo ciEnglish = new CultureInfo("en-US");
        
        // Options for user to choose
        private static string OrderStatusOption = rmRootDialog.GetString("OrderStatusOption", ciEnglish);
        private static string ServiceRepresentative = rmRootDialog.GetString("ServiceRepresentative", ciEnglish);
        private static string MoreOptions = rmRootDialog.GetString("MoreOptions", ciEnglish);

        private readonly IDialogFactory dialogFactory;

        public RootDialog(ILuisService service, IDialogFactory dialogFactory)
            : base(service)
        {
            this.dialogFactory = dialogFactory;
         
        }

        [LuisIntent("")]
        [LuisIntent("None")]
        public Task NoneReceived(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            PromptDialog.Choice(
                context,
                this.OnOptionSelected,
                new List<string>() { OrderStatusOption, ServiceRepresentative, MoreOptions },
                String.Format(rmRootDialog.GetString("DontUnderstand", ciEnglish) + " '" + result.Query + "'..." + rmRootDialog.GetString("WhatToDo", ciEnglish)), rmRootDialog.GetString("NotValid", ciEnglish));

            return Task.CompletedTask;
        }

        [LuisIntent("Greeting")]
        public async Task GreetingReceivedAsync(IDialogContext context, LuisResult result)
        {
            // go to main menu with choices - prompt to choose "What would you like to do?"
            await context.PostAsync(rmRootDialog.GetString("Greeting", ciEnglish));
        }

        [LuisIntent("Order")]
        public Task OrderReceivedAsync(IDialogContext context, LuisResult result)
        {
            // store LuisResult in context userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderRoot>(context);
            return Task.CompletedTask;
        }

        [LuisIntent("Order.Number")]
        public Task OrderNumberReceivedAsync(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderNumber>(context);
            return Task.CompletedTask;
        }

        [LuisIntent("Order.Account")]
        public Task OrderAccountReceivedAsync(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderAccount>(context);

            return Task.CompletedTask;
        }

        [LuisIntent("Order.Date")]
        public Task OrderDateReceivedAsync(IDialogContext context, LuisResult result)
        {
            // store LuisResult in cotext userData
            context.UserData.SetValue<LuisResult>("LuisResult", result);

            StartDialogWithResumeAfter<OrderDate>(context);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context)
        {
            // clear the LUIS entities from userData
            context.UserData.RemoveValue("LuisResult");

            await context.PostAsync(rmRootDialog.GetString("WhatElse", ciEnglish));
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
                await context.PostAsync(rmRootDialog.GetString("TooManyAttempts", ciEnglish));
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
                await context.PostAsync(rmRootDialog.GetString("FailedWithMessage", ciEnglish) + ": " + ex.Message);
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
