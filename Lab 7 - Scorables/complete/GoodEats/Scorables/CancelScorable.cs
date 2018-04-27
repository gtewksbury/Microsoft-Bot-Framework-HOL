using Autofac;
using GoodEats.Dialogs;
using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoodEats.Scorables
{
    public class CancelScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask DialogTask;

        public CancelScorable(IDialogTask dialogTask)
        {
            SetField.NotNull(out this.DialogTask, nameof(dialogTask), dialogTask);
        }

        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            // this is the first method to execute in the scorable workflow
            // this method simply returns the received message if it matches any of the
            // following keywords
            var message = activity as IMessageActivity;
            var values = new string[] { "cancel", "nevermind", "never mind", "forget it", "forgetit" };

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (values.Contains(message.Text, StringComparer.InvariantCultureIgnoreCase))
                {
                    // if the user typed any of the above commands, this scorable
                    // will be scored and potentially executed prior to any dialogs
                    return message.Text;
                }
            }

            return null;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            // each registered scorable (through the ScoreableModule in this solution) will be
            // executed and scored (between 0 and 1).  The scorable with the highest score will be executed
            return state != null;
        }

        protected override double GetScore(IActivity item, string state)
        {
            // if we matched the cancel keywords exectly, give this the top score of 1
            return 1.0;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
            // if this scorable wins and executes, simply call the EndOfConversation dialog
            // which in turn will end the conversation.  I couldn't find a way of successfully
            // ending the conversation directly from here, so I push it to the dialog
            await this.DialogTask.Forward(new EndConversationDialog(), null, item as IMessageActivity, token);
        }


        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}