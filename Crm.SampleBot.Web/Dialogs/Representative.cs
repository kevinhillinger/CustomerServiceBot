using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Crm.SampleBot.Web.Dialogs
{
    [Serializable]
    public class Representative : IDialog<object>
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
                string message = $"getRep for person {result.Entities[0].Entity}";
                await context.PostAsync(message);

                // Call API

            }
            else
            {
                // There is no entity, prompt user for the search paramater
                string message = $"getRep for person PROMPT";
                await context.PostAsync(message);

                // Call API
            }

            // Display results

            context.Done(context);
        }
    }
}