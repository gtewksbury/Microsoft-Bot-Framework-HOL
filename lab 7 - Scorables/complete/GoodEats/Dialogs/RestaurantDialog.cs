using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;
using GoodEats.Services;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class RestaurantDialog : LuisReservationDialog
    {
        private readonly IRestaurantService RestaurantService;

        public RestaurantDialog()
        {
            RestaurantService = new RestaurantService();
        }

        public override async Task StartAsync(IDialogContext context)
        {
            if (context.Restaurant() == null)
            {
                // we don't yet have a restaurant; prompt the user to select a restaurant
                var response = string.Format(Properties.Resources.RESTAURANT_REQUEST, context.Cuisine(), context.Location());
                await PostAsync(context, response);

                // wait for the user to respond with a restaurant
                context.Wait(MessageReceived);
            }
            else if (! await RestaurantService.RestaurantExists(context.Location(), context.Restaurant().Name))
            {
                // we couldn't find the current restaurant in the current location
                // ask the user to provide a different restaurant and wait for a response
                await NotFound(context, context.Location());
            }
            else
            {
                // we already have a restaurant; move onto the when dialog
                context.Call(new WhenDialog(), null);
            }
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
        {
            var response = await item;

            // get a restaurant based on the user's location and their restaurant response
            var restaurant = await RestaurantService.GetRestaurantAsync(context.Location(), response.Text);

            if (restaurant != null)
            {
                // we found a restaurant based on the given response, therefore, store it in state
                context.SetRestaurant(restaurant);

                // send a message to the user confirming their selected restaurant
                var text = string.Format(Properties.Resources.RESTAURANT_CONFIRMATION, restaurant.Name);
                await context.PostAsync(text);

                // pass off to the when dialog
                context.Call(new WhenDialog(), null);
            }
            else
            {
                // we couldn't find a restaurant by the provided name in the user's location
                // give luis a chance to see if the user requested something else
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var restaurant = await activity;

            // we couldn't find the restaurant.  notify the user and wait for a response.
            await NotFound(context, restaurant.Text);
        }

        private async Task NotFound(IDialogContext context, string restaurant)
        {
            // send user a message indicating we didn't find the restaurant
            var text = string.Format(Properties.Resources.RESTAURANT_UNRECOGNIZED, restaurant, context.Location());
            await PostAsync(context, text);

            // wait for the user to respond with another location
            context.Wait(MessageReceived);
        }

        private async Task PostAsync(IDialogContext context, string text)
        {
            // create a new 'card' for each restaurant that we find
            var restaurants = await RestaurantService.GetRestaurantsAsync(context.Location(), context.Cuisine());
            var attachments = restaurants.Select(r => CreateAttachment(r)).ToList();

            // create a new new message including the restaurant cards to be send to the user
            var message = context.MakeMessage();
            message.Text = text;
            message.Attachments = attachments;
            message.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            // send the message to the user
            await context.PostAsync(message);
        }

        private Attachment CreateAttachment(Restaurant restaurant)
        {
            // create a new card view based on the given restaurant
            var card = new ThumbnailCard()
            {
                Title = restaurant.Name,
                Subtitle = $"{restaurant.StreetAddress} {restaurant.City}, {restaurant.State} {restaurant.Zip}",
                Images = new List<CardImage> { new CardImage(url: restaurant.LogoUrl) },
                Buttons = new List<CardAction>{
                    new CardAction() { Title = "More Info", Type = ActionTypes.OpenUrl, Value = restaurant.Url },
                    new CardAction() { Title = "Select", Type = ActionTypes.ImBack, Value = restaurant.Name }}
            };

            return card.ToAttachment();
        }
    }
}