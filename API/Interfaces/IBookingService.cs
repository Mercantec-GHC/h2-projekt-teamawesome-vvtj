using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IBookingService
    {

        Task<BookingResponseDto> CreateBooking(CreateBookingDto dto);

        Task<IEnumerable<GetBookingsDto>> GetAllBookings();
        Task<IEnumerable<BookingDto>> GetBookingByUser(int userId);

        Task<BookingResponseDto?> UpdateBookingDatesAsync(int bookingId, DateOnly newCheckIn, DateOnly newCheckOut);
        Task<bool> DeleteBookingById(int id);
        Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId);


    }
}