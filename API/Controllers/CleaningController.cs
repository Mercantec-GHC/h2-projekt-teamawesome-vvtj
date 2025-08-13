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
    public async Task<IEnumerable<RoomToCleanDto>> GetAllRoomsToClean()
    {
        try
        {
            var rooms = await _cleaningService.GetAllRoomsToCleanAsync();
            return rooms;
        }
        catch (Exception ex)
        {
            // Log the exception (not implemented here)
            //return StatusCode(500, $"Internal server error: {ex.Message}");
            return Enumerable.Empty<RoomToCleanDto>(); // Return an empty list in case of error
        }
    }
}
