
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

        public string UserName { get; set; }
        public int RoomId { get; set; }
        public int RoomNumber { get; set; }

        public string HotelName { get; set; }

        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
