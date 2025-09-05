
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainModels.Enums;

namespace DomainModels.Dto
{
    public class CreateBookingDto
    {
        public string UserName { get; set; } = null!;
        public RoomTypeEnum TypeOfRoom { get; set; }
        public string HotelName { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }

        public int GuestsCount { get; set; }
    }
}
