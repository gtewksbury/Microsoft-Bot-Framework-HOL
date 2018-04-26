using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using GoodEats.Models;
using Microsoft.Bot.Connector;
using GoodEats.Services;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class CuisineDialog : IDialog<Reservation>
    {
        private readonly RestaurantService RestaurantService;

        // Constructors

        public CuisineDialog()
        {
            RestaurantService = new RestaurantService();
        }

        // Methods

        public async Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Cuisine()))
            {
                // we don't yet have a cuisine; prompt the user to select a cuisine
                var response = string.Format(Properties.Resources.CUISINE_REQUEST, context.Location());
                await PostAsync(context, response);

                // wait for the user to respond with a cuisine
                context.Wait(MessageReceived);
            }
            else
            {
                // we already have a cuisine; move onto the restaurant dialog
                context.Call(new RestaurantDialog(), null);
            }
        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var cuisine = await item;

            if (await RestaurantService.HasRestaurantsAsync(context.Location(), cuisine.Text))
            {
                // we found restaurants in the given location for the specified cuisine.  
                // therefore, set the cuisine value in state
                context.SetCuisine(cuisine.Text);

                // send message to the user confirming the selected cuisine
                var response = string.Format(Properties.Resources.CUISINE_CONFIRMATION, cuisine.Text, context.Location());
                await context.PostAsync(response);

                // pass off to the restaurant dialog
                context.Call(new RestaurantDialog(), null);
            }
            else
            {
                // send user a message indicating we didn't find restaurants in the provided location
                var response = string.Format(Properties.Resources.CUISINE_UNRECOGNIZED, cuisine.Text);
                await PostAsync(context, response);

                // wait for the user to respond with another location
                context.Wait(MessageReceived);
            }
        }


        private async Task PostAsync(IDialogContext context, string response)
        { 
            // get all cuisines in the current location
            var cuisines = await RestaurantService.GetCuisinesAsync(context.Location());

            // create suggestions for each cuisine
            var cards = cuisines.Select(c => new CardAction { Title = $"{c.Name} ({c.Count})", Value = c.Name, Type = ActionTypes.ImBack }).ToList();

            // create a message with suggested cuisines to the user
            var message = context.MakeMessage();
            message.Text = response;
            message.SuggestedActions = new SuggestedActions { Actions = cards };

            // send suggestions to the user
            await context.PostAsync(message);
        }
    }
}