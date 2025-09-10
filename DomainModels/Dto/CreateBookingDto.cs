
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
        public bool isBreakfast { get; set; } = false;



    }

    public class BookingResponseDto
    {
        public string UserName { get; set; } = string.Empty;
        public string HotelName { get; set; }
        public RoomTypeEnum RoomType { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int GuestsCount { get; set; }
        public int NightsCount { get; set; }
        public decimal? TotalPrice { get; set; }

        public bool IsBreakfast { get; set; }
    }
}
