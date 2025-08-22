

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
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int NightsCount { get; set; }
        public int GuestsCount { get; set; }
        public decimal? TotalPrice { get; set; }

        //public string? Payment { get; set; }
        //public bool IsPaid { get; set; }
    }


    public class UpdateDatesDto
    {
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
    }
}
