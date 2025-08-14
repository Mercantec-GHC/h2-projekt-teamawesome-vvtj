using API.Interfaces;
using DomainModels.Dto;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomToCleanDto>>> GetAllRoomsToClean()
    {
        try
        {
            var rooms = await _cleaningService.GetAllRoomsToCleanAsync();
            if (rooms == null || !rooms.Any())
            {
                return NotFound("No rooms to clean found.");
            }

            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut]
    public  async Task<IActionResult> MarkRoomAsCleaned(List<int> roomNumbers)
    {
        try
        {
            if (roomNumbers == null || !roomNumbers.Any())
            {
                return BadRequest("Room numbers cannot be null or empty.");
            }

            var result = await _cleaningService.MarkRoomAsCleanedAsync(roomNumber);
            if (result)
            {
                return NoContent(); 
            }
            else
            {
                return NotFound($"Room with number {roomNumbers} not found or already cleaned.");
            }
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

}
