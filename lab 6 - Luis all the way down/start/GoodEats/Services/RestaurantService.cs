using GoodEats.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace GoodEats.Services
{
    [Serializable]
    public class RestaurantService : IRestaurantService
    {

        /// <summary>
        /// Returns a value indicating if we find restaurants in the given location.
        /// </summary>
        /// <param name="location">typically city and state</param>
        /// <returns></returns>
        public async Task<bool> HasRestaurantsAsync(string location)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            var details = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);
            return details.IsSuccessful && details.Data.Restaurants.Count > 0;
        }

        /// <summary>
        /// Returns a value indicating if we find restaurants for the given location and cuisine
        /// </summary>
        /// <param name="location">typically city and state</param>
        /// <param name="cuisine">values such as mexican, chinese, etc.</param>
        /// <returns></returns>
        public async Task<bool> HasRestaurantsAsync(string location, string cuisine)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            var details = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);
            return details.IsSuccessful && details.Data.Restaurants.Any(r => r.FoodTypes.Any(f => f.Equals(cuisine, StringComparison.CurrentCultureIgnoreCase)));
        }

        /// <summary>
        /// Retrieves a collection of restaurants based on the given location and cuisine
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cuisine"></param>
        /// <returns></returns>
        public async Task<List<Restaurant>> GetRestaurantsAsync(string location, string cuisine)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            var details = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);
            return details.Data.Restaurants.Where(r => r.FoodTypes.Any(f => f.Equals(cuisine, StringComparison.CurrentCultureIgnoreCase))).ToList();
        }

        /// <summary>
        /// Returns the restaurant details for the given location and restaurant name
        /// </summary>
        /// <param name="location"></param>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        public async Task<Restaurant> GetRestaurantAsync(string location, string restaurant)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            request.AddParameter("search", restaurant);
            var details = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);
            return details.Data.Restaurants.Where(r => r.Name == restaurant).FirstOrDefault();
        }

        /// <summary>
        /// Returns a value indicating if a we find a restaurant based on the given location
        /// and restaurant name
        /// </summary>
        /// <param name="location"></param>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        public async Task<bool> RestaurantExists(string location, string restaurant)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            request.AddParameter("search", restaurant);
            var details = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);
            return details.Data.Restaurants.Any(r => r.Name.Equals(restaurant, StringComparison.CurrentCultureIgnoreCase));
        }

        /// <summary>
        /// Retrieves a collection of cuisines that are found across all restaurants
        /// in the user's given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Cuisine>> GetCuisinesAsync(string location)
        {
            var client = new RestClient("https://api.eatstreet.com/publicapi/v1");
            var request = CreateRequest();
            request.AddParameter("street-address", location);
            var response = await client.ExecuteGetTaskAsync<RestaurantSearchResults>(request);

            var cuisines = response.Data.Restaurants
                .SelectMany(r => r.FoodTypes)
                .GroupBy(g => g)
                .Select(c => new Cuisine { Name = c.Key, Count = c.Count() })
                .OrderByDescending(o => o.Count);
            return cuisines;
        }

        /// <summary>
        /// Sets up the base settings for all requests.
        /// </summary>
        /// <returns></returns>
        private RestRequest CreateRequest()
        {
            var request = new RestRequest("/restaurant/search", Method.GET);
            var apiKey = ConfigurationManager.AppSettings["EatStreetApiKey"];
            request.AddParameter("method", "both");
            request.AddParameter("pickup-radius", 20);
            request.AddHeader("X-Access-Token", apiKey);
            return request;
        }
    }
}