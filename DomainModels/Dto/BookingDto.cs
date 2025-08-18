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

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        public int RoomId { get; set; }
        public string RoomType { get; set; } = null!;

        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NightsCount { get; set; }
        public int GuestsCount { get; set; }
        public double TotalPrice { get; set; }

        public string? Payment { get; set; }
        public bool IsPaid { get; set; }
    }
}
