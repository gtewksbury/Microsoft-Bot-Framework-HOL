using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoodEats
{
    public static class StateExtensions
    {
        public const string LOCATION = "location";
        public const string RESTAURANT = "restaurant";
        public const string CUISINE = "cuisine";
        public const string WHEN = "when";
        public const string PARTY_SIZE = "size";

        /// <summary>
        /// Gets the current location stored against the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Location(this IBotData context)
        {
            return context.PrivateConversationData.GetValueOrDefault<string>(LOCATION);
        }

        /// <summary>
        /// Gets the current cuisine stored against the conversation state.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string Cuisine(this IBotData context)
        {
            return context.PrivateConversationData.GetValueOrDefault<string>(CUISINE);
        }

        /// <summary>
        /// Gets the current restaurant stored against the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static Restaurant Restaurant(this IBotData context)
        {
            return context.PrivateConversationData.GetValueOrDefault<Restaurant>(RESTAURANT);
        }

        /// <summary>
        /// Gets the current date / time stored against the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static DateTime When(this IBotData context)
        {
            return context.PrivateConversationData.GetValueOrDefault<DateTime>(WHEN);
        }

        /// <summary>
        /// Gets the current party size stored against the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int PartySize(this IBotData context)
        {
            return context.PrivateConversationData.GetValueOrDefault<int>(PARTY_SIZE);
        }

        /// <summary>
        /// Gets a value indicating whether a valid reservation date is stored in the conversation state.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool HasWhen(this IBotData context)
        {
            return context.PrivateConversationData.ContainsKey(WHEN);
        }

        /// <summary>
        /// Gets a value indicating whether a valid party size is stored in the conversation state.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool HasPartySize(this IBotData context)
        {
            return context.PrivateConversationData.ContainsKey(PARTY_SIZE);
        }

        /// <summary>
        /// Sets the given location within the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="location"></param>
        public static void SetLocation(this IBotData context, string location)
        {
            context.PrivateConversationData.SetValue(LOCATION, location);
            context.PrivateConversationData.RemoveValue(RESTAURANT);
        }

        /// <summary>
        /// Gets the given cuisine wihtin the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cuisine"></param>
        public static void SetCuisine(this IBotData context, string cuisine)
        {
            context.PrivateConversationData.SetValue(CUISINE, cuisine);
            context.PrivateConversationData.RemoveValue(RESTAURANT);
        }

        /// <summary>
        /// Gets the given restaurant within the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="restaurant"></param>
        public static void SetRestaurant(this IBotData context, Restaurant restaurant)
        {
            context.PrivateConversationData.SetValue(RESTAURANT, restaurant);
        }

        /// <summary>
        /// Sets the given reservation date within the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="when"></param>
        public static void SetWhen(this IBotData context, DateTime when)
        {
            context.PrivateConversationData.SetValue(WHEN, when);
        }

        /// <summary>
        /// Sets the given party size wihtin the conversation state
        /// </summary>
        /// <param name="context"></param>
        /// <param name="partySize"></param>
        public static void SetPartySize(this IBotData context, int partySize)
        {
            context.PrivateConversationData.SetValue(PARTY_SIZE, partySize);
        }
    }
}