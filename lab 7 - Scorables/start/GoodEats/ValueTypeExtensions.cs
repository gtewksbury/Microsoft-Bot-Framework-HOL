using Chronic;
using Ploeh.Numsense.ObjectOriented;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoodEats
{
    public static class StringExtensions
    { 
        /// <summary>
        /// Attempts to parse a string into an integer.  Uses the Phoeh library
        /// to interpret natural language numbers (such as 'two').
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int? ToInteger(this string value)
        {
            if (int.TryParse(value, out var value1))
            {
                return value1;
            }

            if (Numeral.English.TryParse(value.Replace(' ', '-'), out var value2))
            {
                return value2;
            }

            return null;
        }

        /// <summary>
        /// Attempts to parse a date time value from a string.  Uses the Chronic library
        /// to interpret natural language dates (Such as 'tomorrow at 8 pm')
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime? ToDateTime(this string value)
        {
            value = value.ToUpper().Replace("PM", " PM").Replace("AM", " AM");
            var parser = new Parser();
            return parser.Parse(value)?.Start;
        }
    }
}