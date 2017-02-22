using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crm.SampleBot.Dialogs
{
    public interface IDialogFactory
    {
        IDialog<object> Create<TDialog>() where TDialog : IDialog<object>;
    }
}
