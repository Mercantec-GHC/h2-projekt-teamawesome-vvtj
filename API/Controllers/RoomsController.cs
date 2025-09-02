using DomainModels.Models;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly RoomService _roomService;

    public RoomsController(AppDBContext context, RoomService roomService)
    {
        _context = context;
        _roomService = roomService;
    }

    //Everybody  -> Guests shouldn't be able to see rooms, so not everybody
    /// <summary>
    /// Shows all rooms
    /// </summary>
    /// <returns> A list of rooms</returns>
    /// <response code="404">Rooms not found!</response>
    /// 
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomsDto>>> GetRooms()
    {
        var rooms = await _roomService.GetRooms();

        if (rooms == null)
        {
            return NotFound();
        }

        return Ok(rooms);
    }

    //Everybody -> Guests shouldn't be able to see rooms, so not everybody
    /// <summary>
    /// Show one specific room
    /// </summary>
    /// <param name="id">Unique identifier for room</param>
    /// <returns>A room</returns>
    /// <response code="404">Rooms not found!</response>
    /// 
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomsDto>> GetSpecificRoom(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var room = await _roomService.GetRoomByID(id);
        return Ok(room);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateRoom(RoomCreateDto createRoom)
    {
        var newRoom = await _roomService.PostRoom(createRoom);
        if (newRoom == null)
        {
            return BadRequest();
        }

        return Ok(newRoom);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpGet]
    public async Task<ActionResult> GetRoomsByRoomType()
    {
        
    }

}