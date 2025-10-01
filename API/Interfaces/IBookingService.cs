using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IBookingService
    {

        Task<BookingResponseDto> CreateBooking(CreateBookingDto dto, bool preview = false);

        Task<IEnumerable<GetBookingsDto>> GetAllBookings();

        Task<IEnumerable<GetAvaliableRoomsDto>> GetAvaliableRoomsAsync(string hotelName, DateOnly from, DateOnly to);
        Task<IEnumerable<BookingByUserDto>> GetBookingByUser(int userId);

        Task<BookingResponseDto?> UpdateBookingDatesAsync(int bookingId, DateOnly newCheckIn, DateOnly newCheckOut);
        Task<bool> DeleteBookingById(int id);
        Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId);

        Task<bool> DeleteMyBooking(int bookingId, int userId);

    }
}