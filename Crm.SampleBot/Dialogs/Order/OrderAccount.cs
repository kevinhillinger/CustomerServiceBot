using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderAccount : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            string message = $"Inside OrderAccount";
            await context.PostAsync(message);

            //call context.done to exit this dialog and go back to the root dialog
            context.Done(context);
        }
    }
}
