
## Reservation Conversation Logic

* **RootDialog**
    * Gather the state provided by the user's initial request and *Call* the *LocationDialog*
* **LocationDialog**     
    * *StartAsync*
        * IF the location was NOT retrieved from the original request, ask the user for their preferred location and *Wait* for their response
        * OTHERWISE, *Call* the *CuisineDialog*
    * *MessageReceivedAsync*
        * IF the user-provided location is valid, save to state and *Call* the *CuisineDialog*
        * OTHERWISE, notify the user that the location was not found, ask for another location, and *Wait* for their response
* **CuisineDialog**     
    * *StartAsync*
        * IF the cuisine was NOT retrieved from the original request, ask the user for their preferred cuisine and *Wait* for their response
        * OTHERWISE, *Call* the *RestaurantDialog*
    * *MessageReceivedAsync*
        * IF the user-provided cusine is valid, save to state and *Call* the *RestaurantDialog*
        * OTHERWISE, notify the user that the cuisine was not found, ask for another cuisine, and *Wait* for their response  
* **RestaurantDialog**     
    * *StartAsync*
        * Ask the user for their preferred restaurant and *Wait* for their response
    * MessageReceivedAsync
        * IF the user-provided restaurant is valid, save to state and *Call* the *WhenDialog*
        * OTHERWISE, notify the user that the restaurant was not found, ask for another restaurant, and *Wait* for their response            
* **WhenDialog**     
    * *StartAsync*
        * IF the reservation date / time was NOT retrieved from the original request, ask the user for their preferred time and *Wait* for their response
        * OTHERWISE, *Call* the *PartySizeDialog*
    * *MessageReceivedAsync*
        * IF the user-provided date / time is valid, save to state and *Call* the *PartySizeDialog*
        * OTHERWISE, notify the user that the provided date / time is invalid, ask for a date / time, and *Wait* for their response  
* **PartySizeDialog**     
    * *StartAsync*
        * IF the party size was NOT retrieved from the original request, ask the user for their preferred number of people and *Wait* for their response
        * OTHERWISE, *Call* the *ConfirmReservationDialog* and register a *Done* handler
    * *MessageReceivedAsync*
        * IF the user-provided party size is valid, save to state and *Call* the *ConfirmReservationDialog* and register a *Done* handler
        * OTHERWISE, notify the user that the provided party size is invalid, ask for the party size, and *Wait* for their response  
    * *ConfirmationDoneHandler*
        * Notify the user that their reservation has been booked and end the conversation   
* **ConfirmReservationDialog**     
    * *StartAsync*
        * Ask the user to confirm the reservation and *Wait* for their response
    * *MessageReceivedAsync*
        * IF the user-provided a valid confirmation, save to state and call *Done*
        * OTHERWISE, ask the user to confirm their reservation and *Wait* for their response  

## Create State-Specific Dialogs

### Reservation Location Dialog
Create a new class or code file called *LocationDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

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
```

### Reservation Cuisine Dialog
Create a new class or code file called *CuisineDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

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
                var response = string.Format(Properties.Resources.CUISINE_UNRECOGNIZED, cuisine);
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
```

### Reservation Restaurant Dialog
Create a new class or code file called *RestaurantDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;
using GoodEats.Services;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class RestaurantDialog : IDialog<Reservation>
    {
        private readonly RestaurantService RestaurantService;

        public RestaurantDialog()
        {
            RestaurantService = new RestaurantService();
        }

        public async Task StartAsync(IDialogContext context)
        {

            // we don't yet have a restaurant; prompt the user to select a restaurant
            var response = string.Format(Properties.Resources.RESTAURANT_REQUEST, context.Cuisine(), context.Location());
            await PostAsync(context, response);

            // wait for the user to respond with a restaurant
            context.Wait(MessageReceived);

        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // send user a message indicating we didn't find the restaurant
                var text = string.Format(Properties.Resources.RESTAURANT_UNRECOGNIZED, restaurant, context.Location());
                await PostAsync(context, text);

                // wait for the user to respond with another location
                context.Wait(MessageReceived);
            }
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
```

> Comments

### Reservation Date / Time Dialog
Create a new class or code file called *WhenDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using GoodEats.Models;
using Microsoft.Bot.Connector;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class WhenDialog : IDialog<Reservation>
    {
        public async Task StartAsync(IDialogContext context)
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

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // send the user a message indicating we didn't recognize the date time they entered
                await context.PostAsync(Properties.Resources.WHEN_UNRECOGNIZED);

                // wait for the user to respond with another date / time
                context.Wait(MessageReceived);
            }
        }
    }
}
```

### Reservation Party Size Dialog
Create a new class or code file called *PartySizeDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class PartySizeDialog : IDialog<Reservation>
    {
        public async Task StartAsync(IDialogContext context)
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

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // send the user a message indicating we didn't recognize the party size they entered
                await context.PostAsync(Properties.Resources.PARTY_UNRECOGNIZED);

                // wait for the user to respond with another party size
                context.Wait(MessageReceived);
            }
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
```

### Reservation Date / Time Dialog
Create a new class or code file called *ConfirmReservationDialog.cs* in the Dialogs directory and replace with the following code:

```csharp

using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class ConfirmReservationDialog : IDialog<Reservation>
    {
        public async Task StartAsync(IDialogContext context)
        {
            // send a confirmation message to the user
            await PostAsync(context, Properties.Resources.RESERVATION_CONFIRMATION);

            // wait for the user to respond
            context.Wait(MessageReceived);
        }

        protected async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // send the user a message indicated we didn't understand their response
                await PostAsync(context, Properties.Resources.CONFIRMATION_UNRECOGNIZED);

                // wait for the user to respond with a confirmation
                context.Wait(MessageReceived);
            }
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
```