    using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GoodEats.Models
{
    public class RestaurantSearchResults
    {
        [JsonProperty("restaurants")]
        public List<Restaurant> Restaurants { get; set; }
    }
}