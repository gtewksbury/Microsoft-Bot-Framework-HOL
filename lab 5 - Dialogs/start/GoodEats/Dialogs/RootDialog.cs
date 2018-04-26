using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using GoodEats.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;

namespace GoodEats.Dialogs
{
    [Serializable]
    [LuisModel("12e1e7da-4bf4-42b1-b3e2-0da78f7814e3", "1f7769f4b8f9477bb15ba7c3b5a9a8ad")]
    public class RootDialog : LuisDialog<Reservation>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send a message to the user indicating the request was not recognized
            await context.PostAsync("Sorry, I don't understand.  I only know how to make restaurant reservations.");

            // wait for the next user request
            context.Wait(MessageReceived);
        }

        [LuisIntent("Create Reservation")]
        public async Task CreateReservation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Looks like your attempting to create a reservation.  Let's see what information we were able to pull");

            // attempt to parse the location from the request if provided by the user
            if (result.TryFindEntity("RestaurantReservation.Address", out var locationRecommendation))
            {
                context.PrivateConversationData.SetValue("LOCATION", locationRecommendation.Entity);
            }

            // attempt to parse a cuisine from the luis result if provided by the user
            if (result.TryFindEntity("RestaurantReservation.Cuisine", out var cuisineRecommendation))
            {
                context.PrivateConversationData.SetValue("CUISINE", cuisineRecommendation.Entity);
            }

            // attempt to parse the date time if provided by the user
            if (result.TryFindDateTime("builtin.datetimeV2.datetime", out var when))
            {
                context.PrivateConversationData.SetValue("WHEN", when.Value.ToString());
            }

            // attempt to parse the number of people if provided by the user
            if (result.TryFindInteger("builtin.number", out var partySize))
            {
                context.PrivateConversationData.SetValue("PARTY_SIZE", partySize.Value);
            }
            
            // reply with the parsed location if we saved in state
            if (context.PrivateConversationData.ContainsKey("LOCATION"))
            {
                await context.PostAsync($"Location Preference:  {context.PrivateConversationData.GetValueOrDefault<string>("LOCATION")}");
            }

            // reply with the parsed cuisine if we saved in state
            if (context.PrivateConversationData.ContainsKey("CUISINE"))
            {
                await context.PostAsync($"Cuisine Preference:  {context.PrivateConversationData.GetValueOrDefault<string>("CUISINE")}");
            }

            // reply with the parsed date / time if we saved in state
            if (context.PrivateConversationData.ContainsKey("WHEN"))
            {
                await context.PostAsync($"Date Preference:  {context.PrivateConversationData.GetValueOrDefault<DateTime>("WHEN")}");
            }

            // reply with the parsed number of people if saved in state
            if (context.PrivateConversationData.ContainsKey("PARTY_SIZE"))
            {
                await context.PostAsync($"Party Size Preference:  {context.PrivateConversationData.GetValueOrDefault<int>("PARTY_SIZE")}");
            }

            // end the conversation
            context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
        }
    }
}