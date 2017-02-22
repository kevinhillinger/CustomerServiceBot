using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs.Order
{
    [Serializable]
    class OrderDate : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            string message = $"Inside OrderDate";
            await context.PostAsync(message);
        }
    }
}
