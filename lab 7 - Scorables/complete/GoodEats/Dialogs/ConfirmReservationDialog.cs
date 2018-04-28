using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class ConfirmReservationDialog : LuisReservationDialog
    {
        public override async Task StartAsync(IDialogContext context)
        {
            // send a confirmation message to the user
            await PostAsync(context, Properties.Resources.RESERVATION_CONFIRMATION);

            // wait for the user to respond
            context.Wait(MessageReceived);
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var confirmation = await item;

            if (string.Equals(confirmation.Text, "confirm", StringComparison.InvariantCultureIgnoreCase))
            {
                // the user confirmed the reservation
                var restaurant = context.Restaurant();

                // create a new reservation instance
                var reservation = new Reservation
                {
                    Restaurant = restaurant.Name,
                    RestaurantAddress = $"{restaurant.StreetAddress} {restaurant.City}, {restaurant.State} {restaurant.Zip}",
                    When = context.When(),
                    PartySize = context.PartySize()
                };

                // complete the dialog, passing the reservation to the party size dialog's
                // callback method
                context.Done(reservation);
            }
            else
            {
                // we didn't understand the user's response
                // give luis a chance to see if the user requested a change
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send the user a message indicated we didn't understand their response
            await PostAsync(context, Properties.Resources.CONFIRMATION_UNRECOGNIZED);

            // wait for the user to respond with a confirmation
            context.Wait(MessageReceived);
        }

        private async Task PostAsync(IDialogContext context, string text)
        {
            // get the restaurant and reservation date / time from state
            var restaurant = context.Restaurant();
            var when = context.When();

            // build a new hero card which will provide details of the
            // reservation to be confirmed by the user
            var card = new HeroCard
            {
                Title = $"{restaurant.Name} ({context.PartySize()} people)",
                Subtitle = $"{when.ToLongDateString()} at {when.ToLongTimeString()}",
                Text = $"{restaurant.StreetAddress} {restaurant.City}, {restaurant.State} {restaurant.Zip}",
                Images = new List<CardImage> { new CardImage(url: restaurant.LogoUrl) },
                Buttons = new List<CardAction> { new CardAction() { Title = "Reserve", Type = ActionTypes.ImBack, Value = "confirm" } }
            };

            // create a new message including the hero card
            var message = context.MakeMessage();
            message.Text = text;
            message.Attachments = new List<Attachment> { card.ToAttachment() };

            // send hte message to the user
            await context.PostAsync(message);
        }
    }
}