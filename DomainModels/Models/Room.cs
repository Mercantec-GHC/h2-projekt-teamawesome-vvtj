using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models
{
    public class Room : Common
    {

        public int RoomNumber { get; set; }
        //public required int GuestCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsBreakfast { get; set; }
        public DateTime AvailableFrom { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public RoomType RoomType { get; set; } = default!;

        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; } = default!;
        public DateTime? LastCleaned { get; set; }

        public List<Booking>? Bookings { get; set; } = new List<Booking>();
    }
}