using API.Data;
using API.Interfaces;
using DomainModels;
using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]

public class BookingController : ControllerBase
{
    private readonly IBookingInterface _bookingService;
    public BookingController(IBookingInterface bookingService)
    {
        _bookingService = bookingService;
    }



    [HttpPost]
    public async Task<IActionResult> CreateBooking( Booking booking)
    {
        
        return await _bookingService.CreateBooking(booking);
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
        
        return NoContent(); // 204 success
    }
}