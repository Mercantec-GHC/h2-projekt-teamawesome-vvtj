using API.Data;
using API.Interfaces;
using DomainModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
/// <summary>
/// Controller for handling booking-related operations.
/// </summary>
[Route("api/[controller]")]
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
    /// <returns>Returns a success message or an error if the booking cannot be completed.</returns>
    /// <response code="200">OK with <see cref="BookingResponseDto"/>.</response>
    /// <response code="400">Bad Request if the input is invalid or required entities/room are not found.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have permission to access this resource.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
    [HttpPost]
    public async Task<IActionResult> CreateBooking(CreateBookingDto dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.UserName))
        {
            return BadRequest("Invalid booking data.");
        }

       
        if (dto.CheckOut.Date <= dto.CheckIn.Date)
        {
            return BadRequest("Check-out date must be after check-in date.");
        }

     
        try
        {
            var result = await _bookingService.CreateBooking(dto);
            if (result == null)
            {
                return BadRequest("Booking could not be created.");
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
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<GetBookingsDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookings();
        return Ok(bookings);
    }
    /// <summary>
    /// Gets all bookings of a user by user´s ID. Only available to administrators.
    /// </summary>
    /// <param name="id">Requires UserID</param>
    /// <response code="200">Successfully retrieved the bookings.</response>
    /// <response code="400">Bad request – userId was not provided or is invalid.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="403">Forbidden – the user does not have administrator privileges.</response>
    /// <response code="404">Not found – user with specified ID does not exist or has no bookings.</response>
    /// <response code="500">Internal server error – an unexpected error occurred on the server.</response>
     //[Authorize(Roles = "Admin")]
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(int userId)
    {
        var bookings = await _bookingService.GetBookingByUser(userId);
        return Ok(bookings);
    }

    /// <summary>
    /// Allows to update check-in and check-out dates.
    /// </summary>
    /// <param name="id">Requires BookingID</param>
    /// <returns> Returns a success message if the update was successful, or an error message if it failed.</returns>
    /// <response code="200">Booking dates were successfully updated.</response>
    /// <response code="400">Bad request – invalid input data, invalid dates, or booking not found.</response>
    /// <response code="401">Unauthorized – the user is not authenticated.</response>
    /// <response code="404">Not found – booking with the specified ID does not exist.</response>
    /// <response code="500">Internal server error – an unexpected error occurred while processing the request.</response>
    //[Authorize(Roles = "Admin", "Reciptionist", "User")]
    [HttpPut("{id}/dates")]
    public async Task<IActionResult> UpdateDates(int id, [FromBody] UpdateDatesDto dto)
    {
        var success = await _bookingService.UpdateBookingDates(id, dto.CheckIn, dto.CheckOut);
        if (!success) return BadRequest("Booking not updated.");
        return Ok("Booking dates updated successfully.");
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

    [Authorize(Roles = "Admin,Reception")]
    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByHotel(int hotelId)
    {
        var bookings = await _bookingService.GetBookingByHotel(hotelId);
        return Ok(bookings);
    }
}