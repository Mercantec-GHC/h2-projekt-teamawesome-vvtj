using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IBookingInterface
    {

        Task<IActionResult> CreateBooking(Booking booking);

        Task<IEnumerable<BookingDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetBookingByUser(int userId);
   
        Task<BookingDto?> UpdateBooking(int bookingId, BookingDto dto);
        Task<bool> DeleteBookingById(int id);


    }
}
