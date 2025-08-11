using System.ComponentModel.DataAnnotations;

namespace DomainModels
{
    public class Room 
    {
        public int Id { get; set; }
        public required int RoomNumber { get; set; }
        public int GuestCount { get; set; }
        public bool IsAvailable { get; set; }
        public bool isBreakfast { get; set; }
        public DateTime Availablefrom { get; set; }
        public Type RoomType { get; set; }
        public Hotel _Hotel { get; set; }

        //public Booking BookingPeriod { get; set; }
    }
}