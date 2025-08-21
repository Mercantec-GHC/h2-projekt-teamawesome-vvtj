using Microsoft.AspNetCore.Mvc;
using API.Data;
using API.Services;

[ApiController]
[Route("api/[controller]")]
public class RoomTypesController : ControllerBase
{
    private readonly RoomTypeService _roomtypeService;

    public RoomTypesController(RoomTypeService roomtypeService)
    {
        _roomtypeService = roomtypeService;
    }
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
    [HttpPost]
    public async Task<ActionResult> CreateRoomType(RoomTypeDto roomTypeDto)
    {
        var newRoomType = await _roomtypeService.PostRoomType(roomTypeDto);
        if (newRoomType == null)
        {
            return BadRequest();
        }

        return Ok(newRoomType);
    }
}

    