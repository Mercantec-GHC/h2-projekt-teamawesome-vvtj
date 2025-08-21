using API.Data;
using API.Interfaces;
using DomainModels;
using DomainModels.Dto;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;
/// <summary>
/// Controller til håndtering af booking-relaterede operationer.
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
    /// Opretter en ny booking i systemet.
    /// </summary>
    /// <param name="bookingPostDto">Data for den nye booking.</param>
    /// <returns>Den oprettede booking.</returns>
    /// <response code="201">Bookingen blev oprettet succesfuldt.</response>
    /// <response code="400">Ugyldig forespørgsel eller booking overlap.</response>
    /// <response code="401">Ikke autoriseret - manglende eller ugyldig token.</response>
    /// <response code="500">Der opstod en intern serverfejl.</response>
    [HttpPost]
    public async Task<BookingDto> CreateBooking(BookingDto bookingDto)
    {

        if (bookingDto == null)
            throw new ArgumentNullException(nameof(bookingDto), "Booking data is required.");
        var created = await _bookingService.CreateBooking(bookingDto);
        return created;
    }
    /// <summary>
    /// Get all bookings from the system. Only available to administrators and receptionists.
    /// </summary>
    /// <returns>A list of all bookings with user and room information, check-in and out.</returns>
   /// <response code="200">Bookingen blev tilføjet succesfuldt.</response>
    /// <response code="401">Ikke autoriseret - manglende eller ugyldig token.</response>
    /// <response code="403">Forbudt - kun  bruger har adgang.</response>
    /// <response code="500">Der opstod en intern serverfejl.</response>
    //[Authorize(Roles = "Admin", "Receptionist")]
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
     //[Authorize(Roles = "Admin")]
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(int userId)
    {
        var bookings = await _bookingService.GetBookingByUser(userId);
        return Ok(bookings);
    }

    /// <summary>
    /// Allow to change check-in/out dates, amount of guests. Available to administrators, receptionists.
    /// </summary>
    /// <param name="id">Requires BookingID</param>
    /// <returns>An updated booking with new NightsCount, GuestsCount and TotalPrice.</returns>
     //[Authorize(Roles = "Admin", "Reciptionist")]
    [HttpPut]
    public async Task<BookingDto?> UpdateBooking(int bookingId, BookingDto dto)
    {
        var updated = await _bookingService.UpdateBooking(bookingId, dto);
        if (dto == null)
            throw new ArgumentNullException(nameof(dto), "Booking data is required.");

        return updated;
    }
    /// <summary>
    /// Delete a booking from a system.
    /// </summary>
    /// <param name="id">ID should be deleted.</param>
    /// <returns>Confirmation of deletion.</returns>
    /// <response code="204">Bookingen blev slettet succesfuldt.</response>
    /// <response code="401">Ikke autoriseret - manglende eller ugyldig token.</response>
    /// <response code="403">Forbudt - kan kun slette egne bookinger.</response>
    /// <response code="404">Booking med det angivne ID blev ikke fundet.</response>
    /// <response code="500">Der opstod en intern serverfejl.</response>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var success = await _bookingService.DeleteBookingById(id);

        return success ? NoContent() : NotFound(); // 204 success
    }
    /// Gets all of current or future hotel´s booking. Available to administrators, receptionists.
    /// </summary>
    /// <returns>A list of bookings</returns>
    //[Authorize(Roles = "Admin", "Reciptionist")]
    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByHotel(int hotelId)
    {
        var bookings = await _bookingService.GetBookingByHotel(hotelId);
        return Ok(bookings);
    }
}