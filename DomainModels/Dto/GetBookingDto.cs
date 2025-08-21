using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainModels.Dto
{
    public class GetBookingsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        public int RoomId { get; set; }
        public string RoomType { get; set; } = null!;

        public int HotelId { get; set; }
        public string HotelName { get; set; } = null!;

        public int GuestsCount { get; set; } = 1;

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
