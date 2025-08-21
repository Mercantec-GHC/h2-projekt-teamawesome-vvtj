using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IBookingService
    {

        Task<BookingDto> CreateBooking(BookingDto bookingDto);

        Task<IEnumerable<GetBookingsDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetBookingByUser(int userId);
   
        Task<BookingDto?> UpdateBooking(int bookingId, BookingDto dto);
        Task<bool> DeleteBookingById(int id);
        Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId);


    }
}
