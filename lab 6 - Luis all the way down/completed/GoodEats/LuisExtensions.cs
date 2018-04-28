using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GoodEats
{
    public static class LuisExtensions
    {
        /// <summary>
        /// Attempts to parse a date value from the given LUIS entity type
        /// </summary>
        /// <param name="result"></param>
        /// <param name="type"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static bool TryFindDateTime(this LuisResult result, string type, out DateTime? date)
        {
            date = null;

            if (result.TryFindEntity(type, out var recommendation))
            {

                var resolutionValues = (IList<object>)recommendation.Resolution["values"];
                foreach (var value in resolutionValues)
                {
                    date = Convert.ToDateTime(((IDictionary<string, object>)value)["value"]);
                }
            }

            return date.HasValue;
        }

        /// <summary>
        /// Attempts to parse an integer value from the given LUIS entity type
        /// </summary>
        /// <param name="result"></param>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TryFindInteger(this LuisResult result, string type, out int? value)
        {
            value = null;

            // this is actually an enhancement to an issue with the basic TryFindEntity extension method in the bot framework
            // where it pulls the first number it finds without looking for other entities that have already claimed 
            // that value
            Func<EntityRecommendation, IList<EntityRecommendation>, bool> doesNotOverlapRange = (current, recommendations) =>
            {
                return !recommendations.Where(r => current != r)
                            .Any(r => r.StartIndex.HasValue && r.EndIndex.HasValue && current.StartIndex.HasValue && Enumerable.Range(r.StartIndex.Value, r.EndIndex.Value - r.StartIndex.Value + 1)
                                        .Contains(current.StartIndex.Value));
            };

            // find the recommended entity
            var recommendation = result.Entities.Where(e => e.Type == type && doesNotOverlapRange(e, result.Entities)).FirstOrDefault();

            // attempt to parse an integer value from the result
            if (recommendation != null && int.TryParse(recommendation.Resolution["value"].ToString(), out var val))
            {
                value = val;
            }

            return value.HasValue;
        }
    }
}