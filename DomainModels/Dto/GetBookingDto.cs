
using DomainModels.Enums;

namespace DomainModels.Dto
{
    public class GetBookingsDto
    {
        public int Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string UserName { get; set; }
        public int RoomId { get; set; }
        
        public string HotelName { get; set; }

        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
    }


    public class GetAvaliableRoomsDto
    {

       public int RoomId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public int RoomNumber { get; set; }

        public RoomTypeEnum RoomType { get; set; }
    }
}
