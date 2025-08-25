using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Services;

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
    
    [HttpPut]
     public async Task<ActionResult> UpdateRoomType(RoomTypePutDto roomTypePutDto)
    {
        var updatedRoomType = await _roomtypeService.UpdateRoomType(roomTypePutDto);

        await _context.SaveChangesAsync();

        return Ok(updatedRoomType);
    }
}

    