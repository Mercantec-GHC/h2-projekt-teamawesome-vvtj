using DomainModels.Dto;

namespace Blazor.Models.ViewModels
{
    public class MyBookingsViewModel
    {
        public List<BookingByUserDto> Bookings { get; set; } = new();

        public string? ErrorMessage { get; set; }

        public void SetBookings(IEnumerable<BookingByUserDto> bookings)
        {
            Bookings = bookings.ToList();
        }

        public void SetError(string error)
        {
            ErrorMessage = error;
        }
    }
}
