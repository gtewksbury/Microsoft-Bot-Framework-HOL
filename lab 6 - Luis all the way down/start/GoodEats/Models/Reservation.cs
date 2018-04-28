using System;

namespace GoodEats.Models
{
    public class Reservation
    {
        public string Restaurant { get; set; }

        public string RestaurantAddress { get; set; }
        
        public DateTime When { get; set; }

        public int PartySize { get; set; }

    }
}