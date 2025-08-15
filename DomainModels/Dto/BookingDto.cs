using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public int UserId { get; set; }
        public required DateTime CheckIn { get; set; }
        public required DateTime CheckOut { get; set; }
        public int NightsCount { get; set; }
      
        public int GuestsCount { get; set; }
 //       public double PriceForNight { get; set; }
        public string? Payment { get; set; }
        public bool IsPaid { get; set; }
    }
}
