# Lab 6 - LUIS All the Way Down
Welcome to Lab 6!  At this point, you should have a fully-functioning reservation bot!  However, there are some limitations.  At this point, our bot is very linear.  The user provides a location, preferred cuisine, restaurant, date / time, and party size in that order.  But what if the user wants to go back and change a previous selection?  For example, if prior to confirming the reservation, what if the user wants to change the date and time?  Or, what if the user requests *thai* food and wants to switch to *Italian*?  In these situations, our bot provides a very frustrating user-experience, as their is no way to go back and change previous values.  Go ahead and try it.

Run your bot and ask it to `make a reservation in Pittsburgh`.  When it prompts you for your preferred cuisine, type `sorry, actually make my reservation in Cleveland`.  We're stuck!  Our bot has no idea what's going on!

![Confused Bot](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down/images/bot-confused.png)

Don't worry, we're going to fix that!  Here's one the most important things to understand when designing your bot...**CONVERSATIONS ARE NOT LINEAR**!  We often bounce around between different topics.  Another thing...**USERS AREN'T ROBOTS**!  When the bot asks the user for the number of people on the reservation, what if the user responds `There will be 6 of us`?  Right now, our bot has no idea how to interpret this response, and would again ask the user for the party size.  It's almost like we need some kind of natural language processing when dealing with user responses to our bot...but how?

Oh Yeah, LUIS!

In this lab, we're going to integrate LUIS not only in our *RootDialog*, but with every **Dialog** on the path to making a reservation!

## LuisReservationDialog
Open the starter solution in this lab (or your completed solution from the last lab) in Visual Studio, create a new class or code file named *LuisReservationDialog.cs* in the *Dialogs* directory, and replace its contents with the following code:

```csharp

using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Threading.Tasks;

namespace GoodEats.Dialogs
{
    [Serializable]
    [LuisModel("<LUIS Model Id>", "<LUIS Subscription Key")]
    public abstract class LuisReservationDialog : LuisDialog<Reservation>
    {

        /// <summary>
        /// When implemented, this method will be called when Luis does not
        /// recognize the intent of the user response or request.  For the inheriting dialog,
        /// this provides an opportunity to notify the user that you didn't understand their response
        /// and wait for their next response.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="activity"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [LuisIntent("")]
        [LuisIntent("None")]
        public abstract Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result);

        [LuisIntent("Create Reservation")]
        public async Task CreateReservation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // attempt to parse the reservation location (city, state) if provided and set to state
            if (result.TryFindEntity("RestaurantReservation.Address", out var locationRecommendation))
            {
                context.SetLocation(locationRecommendation.Entity);
            }

            // attempt to parse the cuisine preference if provided and set to state
            if (result.TryFindEntity("RestaurantReservation.Cuisine", out var cuisineRecommendation))
            {
                context.SetCuisine(cuisineRecommendation.Entity);
            }

            // if the user enters a full date (for example, tomorrow night at 9pm), set to state
            if (result.TryFindDateTime("builtin.datetimeV2.datetime", out var date))
            {
                context.SetWhen(date.Value);
            }
            else if (result.TryFindDateTime("builtin.datetimeV2.time", out var time))
            {
                // if the user only enters a time (9pm), we parse the time into the current date
                context.SetWhen(time.Value);
            }

            // if the user provided the number of people, set the value in state
            if (result.TryFindInteger("builtin.number", out var partySize))
            {
                context.SetPartySize(partySize.Value);
            }

            // notify the user that you received their request
            await context.PostAsync(Properties.Resources.CONFIRMATION);

            // pass off to the location dialog to walk through the reservation state chain
            context.Call(new LocationDialog(), null);
        }

        [LuisIntent("Set Reservation Location")]
        public async Task SetReservationLocation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // attempt to parse the reservation location (city, state) if provided and set to state
            if (result.TryFindEntity("RestaurantReservation.Address", out var locationRecommendation))
            {
                // set location state
                context.SetLocation(locationRecommendation.Entity);

                // send a message to the user confirming their new location
                var response = string.Format(Properties.Resources.LOCATION_CONFIRMATION, locationRecommendation.Entity);
                await context.PostAsync(response);
            }

            // pass off to the location dialog to walk through the reservation state chain
            context.Call(new LocationDialog(), null);
        }

        [LuisIntent("Set Reservation Date")]
        public async Task SetReservationDate(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            DateTime? when = null;

            // if the user enters a full date (for example, tomorrow night at 9pm), set to state
            if (result.TryFindDateTime("builtin.datetimeV2.datetime", out var date))
            {
                context.SetWhen(date.Value);
                when = date.Value;
            }
            else if (result.TryFindDateTime("builtin.datetimeV2.time", out var time))
            {
                // if the user only enters a time (9pm), we parse the time into the current date
                context.SetWhen(time.Value);
                when = time.Value;
            }

            // send message to the user confirming their newly selected reservation date and time
            var response = string.Format(Properties.Resources.WHEN_CONFIRMATION, when.Value.ToLongDateString(), when.Value.ToLongTimeString());
            await context.PostAsync(response);

            // pass off to the location dialog to walk through the reservation state chain
            context.Call(new LocationDialog(), null);
        }

        [LuisIntent("Set Reservation Cuisine")]
        public async Task SetReservationCuisine(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // attempt to parse the cuisine preference if provided and set to state
            if (result.TryFindEntity("RestaurantReservation.Cuisine", out var cuisineRecommendation))
            {
                // set cuisine state
                context.SetCuisine(cuisineRecommendation.Entity);

                // send the user a message confirming the new cuisine selection
                var text = string.Format(Properties.Resources.CUISINE_CONFIRMATION, cuisineRecommendation.Entity, context.Location());
                await context.PostAsync(text);
            }

            // pass off to the location dialog to walk through the reservation state chain
            context.Call(new LocationDialog(), null);
        }

        [LuisIntent("Set Reservation Party Size")]
        public async Task SetResevationPartySize(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // if the user provided the number of people, set the value in state
            if (result.TryFindInteger("builtin.number", out var partySize))
            {
                // set party size state
                context.SetPartySize(partySize.Value);

                // send the user a confirmation of the new party size selection
                await context.PostAsync(Properties.Resources.PARTY_SIZE_CONFIRMATION);
            }

            // pass off to the location dialog to walk through the reservation state chain
            context.Call(new LocationDialog(), null);
        }
    }
}
```

> Don't forget to set your *LUIS Model Id* and *LUIS Subscription Key* in the *LuisModelAttribute*.  The same goes for the *RootDialog* if you're working from the starter solution in this lab.

This will serve as the base class for following **Dialogs**:

1. *ConfirmationReservationDialog*
2. *CuisineDialog*
3. *LocationDialog*
4. *PartySizeDialog*
5. *RestaurantDialog*
6. *WhenDialog*

Upon reviewing the code, you'll notice a couple things:

1. Along with our *Create Reservation* intent, *LuisReservationDialog* handles additional intents we have yet to create.
2. We have an abstract *None* intent handler.  Looks like our other **Dialogs** will need to implement this method.

Next we're going to update our **Dialogs** to inherit from our new *LuisReservationDialog* (along with a couple of small enhancements).

#### ConfirmationReservationDialog
Copy the following code into your *ConfirmReservationDialog.cs* file:

```csharp

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
```

#### CuisineDialog
Copy the following code into your *CuisineDialog.cs* file:

```csharp

using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Services;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    public class CuisineDialog : LuisReservationDialog
    {
        private readonly IRestaurantService RestaurantService;

        // Constructors

        public CuisineDialog()
        {
            RestaurantService = new RestaurantService();
        }

        // Methods

        public override async Task StartAsync(IDialogContext context)
        {
            if (string.IsNullOrWhiteSpace(context.Cuisine()))
            {
                // we don't yet have a cuisine; prompt the user to select a cuisine
                var response = string.Format(Properties.Resources.CUISINE_REQUEST, context.Location());
                await PostAsync(context, response);

                // wait for the user to respond with a cuisine
                context.Wait(MessageReceived);
            }
            else if (!await RestaurantService.HasRestaurantsAsync(context.Location(), context.Cuisine()))
            {
                // we couldn't find any restaurants for the given location and cuisine
                // ask the user to provide a different cuisine
                await NotFound(context, context.Cuisine());
            }
            else
            {
                // we already have a cuisine; move onto the restaurant dialog
                context.Call(new RestaurantDialog(), null);
            }
        }

        protected override async Task MessageReceived(IDialogContext context, IAwaitable<IMessageActivity> item)
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
                // we couldn't find a restaurant for the given location and cuisine
                // give luis a chance to see if the user requested something else
                await base.MessageReceived(context, item);
            }
        }

        public override async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var cuisine = await activity;

            // we couldn't find any restaurants serving the requested cuisine in the curent location.  
            // notify the user and wait for a response.
            await NotFound(context, cuisine.Text);
        }

        private async Task NotFound(IDialogContext context, string cuisine)
        {
            // send user a message indicating we didn't find restaurants in the provided location
            var response = string.Format(Properties.Resources.CUISINE_UNRECOGNIZED, cuisine);
            await PostAsync(context, response);

            // wait for the user to respond with another location
            context.Wait(MessageReceived);
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

#### LocationDialog
Copy the following code into your *LocationDialog.cs* file:

```csharp

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
            else if (!await RestaurantService.HasRestaurantsAsync(context.Location()))
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
```

#### PartySizeDialog
Copy the following code into your *PartySizeDialog.cs* file:

```csharp

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
```

#### WhenDialog
Copy the following code into your *WhenDialog.cs* file:

```csharp

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
```

## Updated Converation Flow
Here's the basic blueprint for the updated conversational logic within the above **Dialogs**:
   
* *StartAsync*
	* IF the state is NOT set for the given dialog, prompt the user for the appropriate information and *Wait* for a response
	* IF the state IS currently set but is NOT valid, notify the user by asking them to provide another value and *Wait* for a response
	* IF the state is set valid for the given dialog, *Call* the next dialog in the chain
* *MessageReceivedAsync*
	* IF the user provided a valid value for the given dialog, set the state, and *Call* the next dialog in the chain
	* OTHERWISE, call the base *LuisReservationDialog's* *MessageReceivedAsync* handler, passing the user's response
* *LuisReservationDialog.MessageReceivedAsync*
	* Pass the user's response to LUIS and attempt to identify the user's **Intent** and provided **entities**
	* IF an appropriate **intent** handler IS found on the *LuisReservationDialog*, pass the parsed LuisResult to the handler
		* The handler will store the parsed **entities** in state and *Call* the *LocationDialog*, pushing the converation back through our chain
	* If an **intent** handler is NOT found on *LuisReservationDialog*, invoke the *None* handler, which when implemented by the parent dialog, notifies the user that the request was not understand by asking them to provide another value, and *Wait* for a response 

Alright, let's try this again.  Run your solution and ask your bot to `make a reservation at a pizza restaurant in Pittsburgh`.  When it prompts for your preferred cuisine, respond by saying `sorry, actually make my reservation in Cleveland`.

![Smarter Bot](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down/images/bot-change-location.png)

> Hopefully you're bot was smart enough to handle this!  if not, it probably needs more training.  In that case, go back to the [Luis Website](https://www.luis.ai) and enter, train, and publish some additional **utterances** (If you need a refresher on doing so, you can refer back to [Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%202%20-%20LUIS).  Here are some examples:
>
* `Actually, I want to make a reservation in Pittsburgh`
* `Sorry, I actually wanted to reserve a table in Miami FL`


At this point, our bot is starting to become pretty powerful, allowing the user to update reservation values at any step in the process!  But how did it work?

In the example above, our *RestaurantDialog* was waiting for the user to select a recommended restaurant.  When you typed `sorry, actually make my reservation in Cleveland`, the bot tried to find a restaurant by this name, but obviously couldn't.  At that point, it passed your response to the base *LuisReservationDialog* which in turn called LUIS to see if it recognized your **intent**.  It identified this as a *Create Reservation* intent and called *LuisReservationDialog.CreateReservation*, which overwrote the location value in state and *called* back up our converation chain.

Alright, let's try something else.  Go back to your emulator, and walk through creating a reservation, all the way up until the bot asked you for the number of people.  When it does, type if `there will be 6 of us`.  Hmmm, maybe our bot isn't as smart as we thought.

![Number of People](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down/images/bot-people-selection.png)

What happened here?  We'll, our *Create Reservation* **intent** isn't (and shouldn't be) trained to handle such a statement, and we don't have any **intents** that has been trained to do so.  Remember our *LuisReservationDialog* contained a number of handlers for **intents** that we've yet to create (more specifically, *Set Reservation Location*, *Set Reservation Date*, *Set Reservation Cuisine*, and *Set Reservation Party Size*)?  Let's create them now!

#### Set Reservation Location Intent
Browse to your [LUIS App](https://www.luis.ai), and create a new **intent** called *Set Reservation Location*.

> If you need a refresher on creating, training, and publishing intents, refer back to [Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%202%20-%20LUIS).

Add some **utterances** representing someone wanting to change their location and map the *RestaurantReservation.Address* entities when required.  Here are some examples:

> actual, I'll be in Houston TX

> sorry, I'm actually near Boston

> could you look in Baltimore

> I forgot, I'm in Pittsburgh

> Forget that, I'll be in San Francisco

> actually, I forgot, I'll be in chicago

> actually, can you look in Los Angeles

> change my location to Cleveland

> sorry, i gave you the wrong location, let's look in Phoenix

> Update my location to Orlando FL

#### Set Reservation Date Intent
Create a new **intent** called *Set Reservation Date*.

Add some utterances representing someone wanting to change their reservation date and map the *datetimeV2* entities when required. Here are some examples:

> actually, let's do tomorrow night at 4:30

> change the date to Monday at 5:30 pm

> sorry, let's do 9:30 next Friday

> tomorrow at 3pm

> on second thought, I'd like to eat tonight at 7

> sorry, I gave you the wrong date, let's do Friday at 9:15 pm

> update my reservation to tomorrow at 4:30 pm

#### Set Reservation Cuisine Intent
Create a new **intent** called *Set Reservation Cuisine*.

Add some utterances representing someone wanting to change preferred cuisine and map the *RestaurantReservation.Cuisine* entities when required. Here are some examples:

> actually, are there any good mexican options

> I'm in the mood for italian

> actually, thai sounds good

> On second thought, I could go for some chinese food

> A restaurant that serves Mediterranean would be nice

> Sorry, let's look for a restaurant that serves burgers

> you know what, I could go for sushi

> let's see if there are any restaurants that serve sandwiches

> nothing great there, how about middle eastern?


#### Set Reservation Party Size Intent
Create a new **intent** called *Set Reservation Party Size*.

Add some utterances representing someone wanting to change the number of people on a reservation and map the *number* entities when required. Here are some examples:

> actually, there will be three of us

> on second thought, there will be 10 of us

> sorry, can you update the reservation to six people

> actually, there will just be 2 of us

> we're expecting 6 people

> eight people

> actually, there are 7 of us

> sorry, we'll have 3 people

> update the reservation for 4 people

#### Train and Publish
Now go ahead and *train* and subsequently *publish* you changes (remember, you need to *publish* after training for you updated model to be exposed via the LUIS REST API).

Let's go back to our bot an try again.  Begin a new reservation request, and when the bot prompts you for the number of people, enter `there will be 6 of us`.

![Smart Number of People](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down/images/bot-smart-number-people.png)

Success!  The request should have been gracefully interpreted as a *Set Reservation Party Size* intent your *LuisReservationDialog*.  Let's try another example.  Start a new conversation and type `Make me a reservation at a good italian restaurant in pittsburgh`.  When the bot prompts you with restaurant recommendations, type `actually, I'm in the mood for chinese`.  Hopefully your bot was able to handle this gracefully as well.

![Smart Number of People](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down/images/bot-smart-cuisine.png)

Hopefully this helps instill the importance of supporting fluid conversations when building your own bot experiences!  Bot we're not quite done...

## Quick Recap
In this lab, we learned some relatively simple design patterns for integrating LUIS at all levels of a **conversation**, making our bots adaptive to fluid requests.  By doing so, we were able to drastically improve the user experience of our bot application.

## Next Steps
There may be times, that you want to globally handle very specific commands regardless of the user's current position in the **conversation** (such as *help* or *cancel*).  In [Lab 7](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%207%20-%20Scorables) we'll introduce the concept of **Scorables** and global message handlers to do exactly that.