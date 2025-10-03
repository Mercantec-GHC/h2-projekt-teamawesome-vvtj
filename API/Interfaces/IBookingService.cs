using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;

namespace API.Interfaces
{
    public interface IBookingService
    {

        /// <summary>
        /// Creates a new booking or previews the booking details without saving, based on the preview flag.
        /// </summary>
        /// <param name="dto">The booking details to create.</param>
        /// <param name="preview">If true, returns a preview of the booking without saving.</param>
        /// <returns>A <see cref="BookingResponseDto"/> containing the booking details.</returns>
        Task<BookingResponseDto> CreateBooking(CreateBookingDto dto, bool preview = false);

        /// <summary>
        /// Retrieves all bookings in the system.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="GetBookingsDto"/> representing all bookings.</returns>
        Task<IEnumerable<GetBookingsDto>> GetAllBookings();

        /// <summary>
        /// Retrieves available rooms for a specified hotel within a given date range.
        /// </summary>
        /// <param name="hotelName">The name of the hotel to search for available rooms.</param>
        /// <param name="from">The start date of the availability range.</param>
        /// <param name="to">The end date of the availability range.</param>
        /// <returns>An enumerable collection of <see cref="GetAvailableRoomsDto"/> representing available rooms.</returns>
        Task<IEnumerable<GetAvailableRoomsDto>> GetAvailableRoomsAsync(string hotelName, DateOnly from, DateOnly to);
        /// <summary>
        /// Retrieves all bookings made by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose bookings are to be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="BookingByUserDto"/> representing the user's bookings.</returns>
        Task<IEnumerable<BookingByUserDto>> GetBookingByUser(int userId);

        /// <summary>
        /// Updates the check-in and check-out dates for an existing booking.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to update.</param>
        /// <param name="newCheckIn">The new check-in date.</param>
        /// <param name="newCheckOut">The new check-out date.</param>
        /// <returns>
        /// A <see cref="BookingResponseDto"/> containing the updated booking details,
        /// or <c>null</c> if the booking could not be found or updated.
        /// </returns>
        Task<BookingResponseDto?> UpdateBookingDatesAsync(int bookingId, DateOnly newCheckIn, DateOnly newCheckOut);
        /// <summary>
        /// Deletes a booking by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the booking to delete.</param>
        /// <returns>
        /// <c>true</c> if the booking was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteBookingById(int id);
        /// <summary>
        /// Retrieves all bookings for a specified hotel.
        /// </summary>
        /// <param name="hotelId">The unique identifier of the hotel whose bookings are to be retrieved.</param>
        /// <returns>An enumerable collection of <see cref="BookingDto"/> representing the bookings for the hotel.</returns>
        Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId);

        /// <summary>
        /// Deletes a booking made by the specified user.
        /// </summary>
        /// <param name="bookingId">The unique identifier of the booking to delete.</param>
        /// <param name="userId">The unique identifier of the user who made the booking.</param>
        /// <returns>
        /// <c>true</c> if the booking was successfully deleted; otherwise, <c>false</c>.
        /// </returns>
        Task<bool> DeleteMyBooking(int bookingId, int userId);

    }
}