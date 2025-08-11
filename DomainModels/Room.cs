using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels
{
    public class Room 
    {
        [Key]
        public int Id { get; set; }
        public required int RoomNumber { get; set; }
        public int GuestCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsBreakfast { get; set; }
        public DateTime Availablefrom { get; set; }

        public int TypeId { get; set; }
        [ForeignKey("TypeId")]
        public RoomType RoomType { get; set; }

        public int HotelId { get; set; }
        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; }

        public int BookingId { get; set; }
        [ForeignKey("BookingId")]
        public Booking BookingPeriod { get; set; }
    }
}