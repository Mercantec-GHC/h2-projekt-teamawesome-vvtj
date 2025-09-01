using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Services;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly RoomTypeService _roomtypeService;

    public RoomTypesController(AppDBContext context, RoomTypeService roomtypeService)
    {
        _roomtypeService = roomtypeService;
        _context = context;
    }

    //Everybody
    /// <summary>
    /// Shows all room types
    /// </summary>
    /// <returns>A list of room types</returns>
    /// <response code="404">Room types not found!</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetRoomTypes()
    {
        var rooms = await _roomtypeService.GetRoomTypes();

        if (rooms == null)
        {
            return NotFound();
        }

        return Ok(rooms);
    }

    //Everybody
    /// <summary>
    /// Show one specific room type
    /// </summary>
    /// <param name="id">Unique identifier for room types</param>
    /// <returns>A room type</returns>
    /// <response code="404">Room type not found!</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeDto>> GetSpecificRoomtype(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await _roomtypeService.GetSpecificRoomType(id);
        return Ok(room);
    }

    /// <summary>
    /// Updates either price or description for a roomtype
    /// </summary>
    /// <param name="id">Unique identifier for room types</param>
    /// <param name="roomTypePutDto">The updated roomtype data</param>
    /// <returns>Updated roomtype</returns>
    /// <response code="200">Room type successfully updated.</response>
    /// <response code="404">Room type not found.</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
     public async Task<ActionResult> UpdateRoomType(int id, RoomTypePutDto roomTypePutDto)
    {
        var updatedRoomType = await _roomtypeService.UpdateRoomType(id, roomTypePutDto);

        await _context.SaveChangesAsync();

        return Ok(updatedRoomType);
    }
}

    