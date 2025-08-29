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

        Task<bool> UpdateBookingDates(int bookingId, DateTime newCheckIn, DateTime newCheckOut);
        Task<bool> DeleteBookingById(int id);
        Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId);


    }
}