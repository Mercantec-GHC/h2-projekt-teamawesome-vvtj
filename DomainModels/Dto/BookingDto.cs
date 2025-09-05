


using DomainModels.Enums;

namespace DomainModels.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
       
        public int UserId { get; set; }
        public string UserName { get; set; }

        public int RoomId { get; set; }
        public RoomTypeEnum RoomType { get; set; }

        public int HotelId { get; set; }
        public string HotelName { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NightsCount { get; set; }
        public int GuestsCount { get; set; }
        public decimal? TotalPrice { get; set; }

        public bool isBreakfast { get; set; }


    }


    public class UpdateDatesDto
    {
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
    }

    public class BookingByUserDto
    {
        public DateTime CreatedAt { get; set; }
        public string UserName { get; set; }
        public string HotelName { get; set; }
        public RoomTypeEnum RoomType { get; set; }
        public DateOnly CheckIn { get; set; }
        public DateOnly CheckOut { get; set; }
        public int NightsCount { get; set; }
        public int GuestsCount { get; set; }
        public decimal? TotalPrice { get; set; }
        public bool IsBreakfast { get; set; }
    }

}
