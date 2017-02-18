using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
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
            if (result != null)
            {
                // There is an entity. Use for the search (assume correct)
                string message = $"getOrderStatus for order #{result.Entities[0].Entity}";
                await context.PostAsync(message);

                // call API

            }
            else
            {
                // There is no entity, prompt user for the search paramater

                string message = $"getOrderStatus for order # PROMPT";
                await context.PostAsync(message);

                // call API
            }

            // Display results


            context.Done(context);
        }
    }
}