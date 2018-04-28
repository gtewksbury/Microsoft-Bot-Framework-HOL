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
    [LuisModel("<LUIS Model Id>", "<LUIS Subscription Key>")]
    public class RootDialog : LuisDialog<Reservation>
    {
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send a message to the user indicating the request was not recognized
            await context.PostAsync(Properties.Resources.NONE);

            // end the conversation
            context.EndConversation(EndOfConversationCodes.Unknown);
        }

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

            // send a message to the user confirming their intent to create a reservation
            await context.PostAsync(Properties.Resources.GREETING);

            // we need to make sure we capture the following information for the reservation:
            // 1. User's location (city, state)
            // 2. User's preferred cuisine (thai, italian, etc.)
            // 3. Restaurant (based on location and cuisine)
            // 4. Date / time of the reservation
            // 5. Number of people

            // we start by first invoking a dialog for requesting the location
            // and in turn call other state-specific dialogs in sequence
            context.Call(new LocationDialog(), null);
        }
    }
}