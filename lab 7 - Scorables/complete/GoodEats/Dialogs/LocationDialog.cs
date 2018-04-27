using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Services;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class LocationDialog : LuisReservationDialog
    {

        private readonly IRestaurantService RestaurantService;

        public LocationDialog()
        {
            RestaurantService = new RestaurantService();
        }

        public override async Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Location()))
            {
                // we don't yet have a location; prompt the user to select a location
                var response = string.Format(Properties.Resources.LOCATION_REQUEST);
                await context.PostAsync(response);

                // wait for the user to respond with their location
                context.Wait(MessageReceived);
            }
            else if (! await RestaurantService.HasRestaurantsAsync(context.Location()))
            {
                // we couldn't find any restaurants for the current location
                // ask the user to provide a different location
                await NotFound(context, context.Location());
            }
            else
            {
                // we already have a location; move onto cuisine
                context.Call(new CuisineDialog(), null);
            }
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var location = await item;

            if (await RestaurantService.HasRestaurantsAsync(location.Text))
            {
                // we found restaurants in the given location, therefore, set the location state
                context.SetLocation(location.Text);

                // send message to the user confirming the selected location
                var response = string.Format(Properties.Resources.LOCATION_CONFIRMATION, location.Text);
                await context.PostAsync(response);

                // pass off to the cuisine dialog
                context.Call(new CuisineDialog(), null);
            }
            else
            {
                // we couldn't find a restaurant for the given location
                // give luis a chance to see if the user requested something else
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var location = await activity;

            // we couldn't find the given location.  notify the user and wait for a response.
            await NotFound(context, location.Text);
        }

        private async Task NotFound(IDialogContext context, string location)
        {
            // send user a message indicating we didn't find restaurants in the provided location
            var response = string.Format(Properties.Resources.LOCATION_UNRECOGNIZED, location);
            await context.PostAsync(response);

            // wait for the user to respond with another location
            context.Wait(MessageReceived);
        }
    }
}