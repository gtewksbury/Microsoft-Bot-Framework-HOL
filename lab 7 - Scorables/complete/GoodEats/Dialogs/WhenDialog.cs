using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class WhenDialog : LuisReservationDialog
    {
        public override async Task StartAsync(IDialogContext context)
        {
            if (context.HasWhen())
            {
                // we already have a data and time, pass off to the party dialog
                context.Call(new PartySizeDialog(), null);
            }
            else
            {
                // we don't yet have a time; prompt the user for a date and time
                await context.PostAsync(Properties.Resources.WHEN_REQUEST);

                // wait for the user response
                context.Wait(MessageReceived);
            }
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var response = await item;

            // attempt to parse the date time using the 'Chronic Parser' library
            var when = response.Text.ToDateTime();

            if (when.HasValue)
            {
                // the user provided a valid date and time, therefore set the when state for the reservation
                context.SetWhen(when.Value);

                // send message to the user confirming the provided date time
                var text = string.Format(Properties.Resources.WHEN_CONFIRMATION, when.Value.ToLongDateString(), when.Value.ToLongTimeString());
                await context.PostAsync(text);

                // pass off to the party size dialog
                context.Call(new PartySizeDialog(), null);
            }
            else
            {
                // we didn't understand the user-provided date / time
                // give luis a chance to see if the user requested something else
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send the user a message indicating we didn't recognize the date time they entered
            await context.PostAsync(Properties.Resources.WHEN_UNRECOGNIZED);

            // wait for the user to respond with another date / time
            context.Wait(MessageReceived);
        }
    }
}