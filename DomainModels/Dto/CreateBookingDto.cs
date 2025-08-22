using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Dto
{
    public class CreateBookingDto
    {
        public string UserName { get; set; } = null!;
        public string TypeOfRoom { get; set; }
        public string HotelName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

        public int GuestsCount { get; set; } 

        public int NightsCount 
        { 
            get 
            {
                return (CheckOut.Date - CheckIn.Date).Days > 0 ? (CheckOut.Date - CheckIn.Date).Days : 1; // At least 1 night
            }
        }

       public decimal TotalPrice { get; set; } 
    }
}
