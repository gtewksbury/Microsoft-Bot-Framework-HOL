using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;
using GoodEats.Services;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class LocationDialog : IDialog<Reservation>
    {

        private readonly RestaurantService RestaurantService;

        public LocationDialog()
        {
            RestaurantService = new RestaurantService();
        }

        public async Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Location()))
            {
                // we don't yet have a location; prompt the user to select a location
                var response = string.Format(Properties.Resources.LOCATION_REQUEST);
                await context.PostAsync(response);

                // wait for the user to respond with their location
                context.Wait(MessageReceived);
            }
            else
            {
                // we already have a location; move onto cuisine
                context.Call(new CuisineDialog(), null);
            }
        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // send user a message indicating we didn't find restaurants in the provided location
                var response = string.Format(Properties.Resources.LOCATION_UNRECOGNIZED, location);
                await context.PostAsync(response);

                // wait for the user to respond with another location
                context.Wait(MessageReceived);
            }
        }
    }
}