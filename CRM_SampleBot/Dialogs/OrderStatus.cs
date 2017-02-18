using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace CRM_SampleBot.Dialogs
{
    [Serializable]
    public class OrderStatus : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // load the LuisResult from context.UserData
            LuisResult result = new LuisResult();
            context.UserData.TryGetValue<LuisResult>("LuisResult", out result);

            // check if LuisResult contains an entity
            if ((result != null) && (result.Entities.Count > 0))
            {
                // There is an entity. Use for the search (assume correct)
                string message = $"getOrderStatus for order #{result.Entities[0].Entity}";
                await context.PostAsync(message);

                // set entity flag to true.. no need to ask user for one
                bool haveEntity = true;
                context.UserData.SetValue<bool>("haveEntity", haveEntity);

                // call API and display results
                context.Wait(CallAPI);
            }
            else
            {
                // There is no entity, prompt user for the search paramater
                await context.PostAsync("What order number would you like to search?");

                // set entity flag to false.. we need to ask user for one
                bool haveEntity = false;
                context.UserData.SetValue<bool>("haveEntity", haveEntity);

                // call API and display results
                context.Wait(CallAPI);
            }
        }
        public async Task CallAPI(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            bool result;
            context.UserData.TryGetValue<bool>("haveEntity", out result);

            if (result)
            {
                // we already have an entity continue with API call using LUIS
                // load the LuisResult from context.UserData
                LuisResult entity = new LuisResult();
                context.UserData.TryGetValue<LuisResult>("LuisResult", out entity);
                var orderNumber = entity.Entities[0].Entity;

                string message = $"getOrderStatus for order #{orderNumber}";
                await context.PostAsync(message);

            } else {
                // we do not have an entity, take it from the argument
                //variable to hold message coming in
                var orderNumber = await argument;

                string message = $"getOrderStatus for order #{orderNumber.Text}";
                await context.PostAsync(message);

            }

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(argument);
            
        }
    }
}