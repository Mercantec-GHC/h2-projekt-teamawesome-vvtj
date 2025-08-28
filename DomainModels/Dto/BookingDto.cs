

using DomainModels.Models;

namespace DomainModels.Dto
{
    public class BookingDto
    {
        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; }

        public int RoomId { get; set; }
        public string RoomType { get; set; }

        public int RoomTypeId { get; set; }

        public string TypeOfRoom { get; set; }

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
}
