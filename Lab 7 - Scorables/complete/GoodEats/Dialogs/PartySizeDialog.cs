using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class PartySizeDialog : LuisReservationDialog
    {
        public override async Task StartAsync(IDialogContext context)
        {
            if (context.HasPartySize())
            {
                // we already have a party size.  pass off to the confirmation dialog
                context.Call(new ConfirmReservationDialog(), ReservationConfirmedAsync);
            }
            else
            {
                // we don't yet have a party size; prompt the user for their party size
                await context.PostAsync(Properties.Resources.PARTY_REQUEST);
                
                // wait for the user response
                context.Wait(MessageReceived);
            }
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var response = await item;

            // attempt to parse the party size based on the phoeh library (can parse '7' or 'seven')
            var partySize = response.Text.ToInteger();

            if (partySize.HasValue)
            {
                // the user provided a valid party size, therefore set the party size state
                context.SetPartySize(partySize.Value);

                // send message to the user confirming the provided party size
                await context.PostAsync(Properties.Resources.CONFIRMATION);

                // pass off to the reservation confirmation dialog
                context.Call(new ConfirmReservationDialog(), ReservationConfirmedAsync);
            }
            else
            {
                // we didn't understand the user-provided party size (int)
                // give luis a chance to see if the user requested something else
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send the user a message indicating we didn't recognize the party size they entered
            await context.PostAsync(Properties.Resources.PARTY_UNRECOGNIZED);

            // wait for the user to respond with another party size
            context.Wait(MessageReceived);
        }

        private async Task ReservationConfirmedAsync(IDialogContext context, IAwaitable<Reservation> response)
        {
            // we received confirmation from the reservation confirmation dialog!
            var reservation = await response;

            // send information to the user with their confirmation details
            var text = string.Format(Properties.Resources.BOOKED_CONFIRMATION, reservation.Restaurant, reservation.When.ToLongDateString(), reservation.When.ToLongTimeString());
            await context.PostAsync(text);

            // end the conversation (this will reset the dialog stack and clear all convesation state)
            context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
        }
    }
}