using GoodEats.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoodEats.Services
{
    public interface IRestaurantService
    {
        /// <summary>
        /// Returns a value indicating if we find restaurants in the given location.
        /// </summary>
        /// <param name="location">typically city and state</param>
        /// <returns></returns>
        Task<bool> HasRestaurantsAsync(string location);

        /// <summary>
        /// Returns a value indicating if we find restaurants for the given location and cuisine
        /// </summary>
        /// <param name="location">typically city and state</param>
        /// <param name="cuisine">values such as mexican, chinese, etc.</param>
        /// <returns></returns>
        Task<bool> HasRestaurantsAsync(string location, string cuisine);

        /// <summary>
        /// Retrieves a collection of restaurants based on the given location and cuisine
        /// </summary>
        /// <param name="location"></param>
        /// <param name="cuisine"></param>
        /// <returns></returns>
        Task<List<Restaurant>> GetRestaurantsAsync(string location, string cuisine);

        /// <summary>
        /// Retrieves a collection of cuisines that are found across all restaurants
        /// in the user's given location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        Task<IEnumerable<Cuisine>> GetCuisinesAsync(string location);

        /// <summary>
        /// Returns the restaurant details for the given location and restaurant name
        /// </summary>
        /// <param name="location"></param>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        Task<Restaurant> GetRestaurantAsync(string location, string restaurant);

        /// <summary>
        /// Returns a value indicating if a we find a restaurant based on the given location
        /// and restaurant name
        /// </summary>
        /// <param name="location"></param>
        /// <param name="restaurant"></param>
        /// <returns></returns>
        Task<bool> RestaurantExists(string location, string restaurant);
    }
}
