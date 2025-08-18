using API.Data;
using API.Interfaces;
using DomainModels;
using DomainModels.Dto;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;
    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }



    [HttpPost]
    public async Task<BookingDto> CreateBooking(BookingDto bookingDto)
    {
        
        if (bookingDto == null)
            throw new ArgumentNullException(nameof(bookingDto), "Booking data is required.");
        var created = await _bookingService.CreateBooking(bookingDto); 
        return created;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetAllBookings()
    {
        var bookings = await _bookingService.GetAllBookings();
        return Ok(bookings);
    }

    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByUser(int userId)
    {
        var bookings = await _bookingService.GetBookingByUser(userId);
        return Ok(bookings);
    }

    [HttpPut]
    public async Task<ActionResult<BookingDto>> UpdateBooking(int bookingId, BookingDto dto)
    {
        var updated = await _bookingService.UpdateBooking(bookingId, dto);
       
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        var success = await _bookingService.DeleteBookingById(id);

        return success ? NoContent() : NotFound(); // 204 success
    }

    [HttpGet("hotel/{hotelId}")]
    public async Task<ActionResult<IEnumerable<BookingDto>>> GetBookingsByHotel(int hotelId)
    {
        var bookings = await _bookingService.GetBookingByHotel(hotelId);
        return Ok(bookings);
    }
}