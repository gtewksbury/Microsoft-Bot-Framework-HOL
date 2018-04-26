using Newtonsoft.Json;
using System.Collections.Generic;

namespace GoodEats.Models
{
    public class RestaurantSearchResults
    {
        [JsonProperty("restaurants")]
        public List<Restaurant> Restaurants { get; set; }
    }
}