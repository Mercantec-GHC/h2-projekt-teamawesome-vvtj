using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Route("api/[controller]")]
[ApiController]
public class CleaningController : ControllerBase
{
    ICleaningService _cleaningService;

    public CleaningController(ICleaningService cleaningService)
    {
        _cleaningService = cleaningService;
    }

    /// <summary>
    /// Retrieves a list of rooms that require cleaning.
    /// </summary>
    /// <remarks>
    /// A room is considered to require cleaning if:
    /// <list type="bullet">
    /// <item>It has never been cleaned,</item>
    /// <item>It was last cleaned 3 or more days ago, or</item>
    /// <item>It has an associated booking with a check-out date up to the current time.</item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// 200 OK — A list of rooms to clean (<see cref="RoomToCleanDto"/>).<br/>
    /// 404 Not Found — If no rooms requiring cleaning were found.<br/>
    /// 500 Internal Server Error — If an unexpected error occurs while processing the request.
    /// </returns>
    /// 
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomToCleanDto>>> GetAllRoomsToClean()
    {
        try
        {
            var rooms = await _cleaningService.GetAllRoomsToCleanAsync();
            if (rooms == null || !rooms.Any())
            {
                return Ok("No rooms to clean found.");
            }

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    /// <summary>
    /// Marks the specified rooms as cleaned by updating their <c>LastCleaned</c> date 
    /// to the current UTC time for the given hotels.
    /// </summary>
    /// <param name="roomNumbers">
    /// A list of <see cref="RoomToCleanDto"/> objects containing hotel IDs and room numbers 
    /// that should be marked as cleaned.  
    /// Must not be null or empty.
    /// </param>
    /// <returns>
    /// <para>
    /// 200 OK — If one or more rooms were successfully marked as cleaned.  
    /// </para>
    /// <para>
    /// 400 Bad Request — If the <paramref name="roomNumbers"/> parameter is null or empty.  
    /// </para>
    /// <para>
    /// 404 Not Found — If none of the specified rooms exist or they were already marked as cleaned.  
    /// </para>
    /// <para>
    /// 500 Internal Server Error — If an unexpected error occurs while processing the request.  
    /// </para>
    /// </returns>
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
	[HttpPut]
    public async Task<IActionResult> MarkRoomAsCleaned(List<RoomToCleanDto> roomNumbers)
    {
        try
        {
            if (roomNumbers == null || !roomNumbers.Any())
            {
                return BadRequest("Room numbers cannot be null or empty.");
            }

            var result = await _cleaningService.MarkRoomAsCleanedAsync(roomNumbers);

            if (result != null && result.Any())
            {
                var cleaned = string.Join(", ",
                    result.SelectMany(r => r.RoomNumbers.Select(n => $"Hotel {r.HotelId} - Room {n}")));

                return Ok($"Rooms marked as cleaned today: {cleaned}");
            }
            else
            {
                var requested = string.Join(", ",
                    roomNumbers.SelectMany(r => r.RoomNumbers.Select(n => $"Hotel {r.HotelId} - Room {n}")));

                return NotFound($"Room(s) not found or already cleaned: {requested}");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
