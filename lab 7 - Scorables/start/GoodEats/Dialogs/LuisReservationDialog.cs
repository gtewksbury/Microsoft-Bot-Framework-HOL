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