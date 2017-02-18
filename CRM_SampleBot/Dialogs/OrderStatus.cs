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
                // store orderNumber
                var orderNumber = result.Entities[0].Entity;
                context.UserData.SetValue<string>("orderNumber", orderNumber);

                // call API and display results
                await CallAPI(context);          
            }
            else
            {
                // There is no entity, prompt user for the search paramater
                await context.PostAsync("What order number would you like to search?");

                // call function to store result
                context.Wait(getEntity);
            }
        }

        public async Task getEntity(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            // we do not have an entity, take it from the argument
            //variable to hold message coming in
            var arg = await argument;
            var orderNumber = arg.Text;

            // store orderNumber
            context.UserData.SetValue<string>("orderNumber", orderNumber);

            // call API and display results
            await CallAPI(context);
        }

        public async Task CallAPI(IDialogContext context)
        {
            // retrieve orderNumber to be searched
            var orderNumber = "";
            context.UserData.TryGetValue<string>("orderNumber", out orderNumber);

            string message = $"getOrderStatus for order #{orderNumber}";
            await context.PostAsync(message);

            // Call API with orderNumber
            message = $"Calling API..";
            await context.PostAsync(message);

            // Display results
            message = $"Displaying API results..";
            await context.PostAsync(message);

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(context);
            
        }
    }
}