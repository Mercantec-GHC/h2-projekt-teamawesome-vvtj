using API.Data;
using API.Interfaces;
using API.Services;
using DomainModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers;
/// <summary>
/// Controller for handling booking-related operations.
/// </summary>
[Route("api/bookings")]
[ApiController]

public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }


    /// <summary>
    /// Creates a new booking in the system.
    /// </summary>
    /// <param name="CreateBookingDto">The booking data from the client.</param>
    /// <param name="preview">
    /// If true, only returns a booking preview (price, dates, etc.)
    /// without saving it to the database or sending confirmation.
    /// If false, creates the booking in the database and sends confirmation.
    /// </param>
    /// <returns>Returns a success message or an error if the booking cannot be completed.</returns>
    /// <response code="200">OK with <see cref="BookingResponseDto"/>.</response>
    /// <response code="400">Bad Request if the input is invalid or required entities/room are not found.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have permission to access this resource.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto, [FromQuery] bool preview = false)
    {

        var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (currentUserId == null)
            return Unauthorized("UserId is not found in token.");

        if (dto == null || string.IsNullOrWhiteSpace(dto.UserName))
        {
            return BadRequest("Invalid booking data.");
        }


        if (dto.CheckOut <= dto.CheckIn)
            return BadRequest("Check-out date must be after check-in date.");

        try
        {
            var result = await _bookingService.CreateBooking(dto, preview);

            if (result == null)
            {
                return BadRequest("No available rooms of this type for the selected dates.");
            }

            return Ok(result);

        }

        catch (Exception ex)
        {
            return StatusCode(500, $"An error occurred: {ex.Message}");
        }
    }
    /// <summary>
    /// Get all bookings from the system. Only available to administrators and receptionists.
    /// </summary>
    /// <returns>A list of all bookings with user and room information, check-in and out.</returns>
    /// <response code="200">OK with list of <see cref="GetBookingsDto"/>.</response>
    /// <response code="400">Bad request – invalid request or parameters.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have permission to access this resource.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [Authorize(Roles = "Admin, Reception")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetBookingsDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookings();
        return Ok(bookings);
    }

    /// <summary>
    /// Get all available rooms in the specified hotel for a given period of time.  
    /// </summary>
    /// <param name="hotelName">Hotel name (case sensitive).Choose one of the hotels: Halo, Eden, Oasis</param>
    /// <param name="from">Start date in format yyyy-MM-dd</param>
    /// <param name="to">End date in format yyyy-MM-dd</param>
    /// <returns>A list of available rooms with their IDs, numbers, hotel name, and room type id.</returns>
    /// <response code="200">OK with list of <see cref="GetAvailableRoomsDto"/>.</response>
    /// <response code="400">Bad request – invalid request or parameters.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have permission to access this resource.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [Authorize(Roles = "Admin, Reception")]
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<GetAvailableRoomsDto>>> GetAvailableRooms(
        [FromQuery] string hotelName,
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to)
    {
        if (string.IsNullOrWhiteSpace(hotelName))
            return BadRequest("hotelName is required.");
        if (to <= from)
            return BadRequest("Invalid date range. Use yyyy-MM-dd and ensure 'to' is after 'from'.");

        var rooms = await _bookingService.GetAvailableRoomsAsync(hotelName, from, to);
        if (!rooms.Any())
        {
           
            return NotFound("No available rooms for the selected period.");
        }

        return Ok(rooms); 
    }
    /// <summary>
    /// Gets all bookings of a user by user´s ID. 
    /// </summary>
    /// <param name="id">Requires UserID</param>
    /// <response code="200">Successfully retrieved the bookings.</response>
    /// <response code="400">Bad request – userId was not provided or is invalid.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have administrator privileges.</response>
    /// <response code="404">Not found – user with specified ID does not exist or has no bookings.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [Authorize]
    [HttpGet("bookings")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdStr is null || !int.TryParse(userIdStr, out var userId))
            return Unauthorized("UserId is not authorized.");

        var bookings = await _bookingService.GetBookingByUser(userId);
        if (bookings is null || !bookings.Any())
            return NotFound("No bookings found for this user.");

        return Ok(bookings);
    }

    /// <summary>
    /// Updates the check-in and check-out dates of an existing booking,
    /// and automatically recalculates nights count and total price.
    /// </summary>
    /// <param name="id">The ID of the booking to update</param>
    /// <param name="dto">The DTO containing the new check-in and check-out dates</param>
    /// <returns>
    /// Returns the updated booking details if successful.
    /// </returns>
    /// <response code="200">Returns the updated booking with new dates, nights count and total price</response>
    /// <response code="400">Invalid input data</response>
    ///  <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="404">Booking with the specified ID not found</response>
    /// <response code="500">Unexpected server error</response>
    [Authorize]
    [HttpPut]
    public async Task<IActionResult> UpdateDates(int id, UpdateDatesDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
            return Unauthorized("UserId is not found in token.");

        if (dto.CheckOut <= dto.CheckIn)
            return BadRequest("Check-out date must be after check-in date.");

        var updatedBooking = await _bookingService.UpdateBookingDatesAsync(id, dto.CheckIn, dto.CheckOut);

        if (updatedBooking == null)
            return BadRequest("Booking could not be updated. Room may be unavailable or booking not found.");

        return Ok(updatedBooking);
    }
    /// <summary>
    /// Delete a booking from a system.
    /// </summary>
    /// <param name="id">BookingID.</param>
    /// <returns>Confirmation of deletion.</returns>
    /// <response code="204">The booking was deleted successfully.</response>
    /// <response code="401">Not authorized - missing or invalid token.</response>
    /// <response code="404">Booking with the specified ID was not found.</response>
    /// <response code="500">>Internal server error.</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var success = await _bookingService.DeleteBookingById(id);

        return success ? NoContent() : NotFound(); // 204 success
    }
    /// <summary>
    /// Gets all of current or future hotel´s booking. Available to administrators, receptionists.
    /// </summary>
    /// <param name="id">HotelId</param>
    /// <returns>A list of <see cref="BookingDto"/> for chosen hotel</returns>
    ///  <response code="400">Bad request - HotelId was not provided or is invalid.</response>
    ///   <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>

    [Authorize(Roles = "Admin, Receptionist")]
    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByHotel(int hotelId)
    {
        var bookings = await _bookingService.GetBookingByHotel(hotelId);
        return Ok(bookings);
    }

    /// <summary>
    /// Deletes a booking belonging to the current user.
    /// </summary>
    /// <param name="id">The ID of the booking to delete.</param>
    /// <returns>
    /// <response code="204"> No Content if the booking was deleted successfully.</response>
    /// <response code="400"> Bad Request if the booking cannot be deleted (does not exist, does not belong to the user, or has already started).</returns>
    ///<response code="401">Unauthorized – the user is not authenticated.</response>
    ///  <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [Authorize]
    [HttpDelete("user/bookings/{id}")]
    public async Task<IActionResult> DeleteMyBooking(int id)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
        {
            return Unauthorized("User is not authorized.");
        }

        var success = await _bookingService.DeleteMyBooking(id, userId);

        if (!success)
        {
            return BadRequest("Unable to delete booking. It may not exist, not belong to you, or has already started.");
        }

        return NoContent();
    }
}