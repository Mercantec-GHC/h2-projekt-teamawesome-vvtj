using DomainModels.Models;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Authorization;
using Grpc.Core;

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
    /// <response code="200">Rooms successfully found!</response>
    /// <response code="404">Could not find rooms!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomsDto>>> GetRooms()
    {
        try
        {
            return Ok(await _roomService.GetRooms());
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch
        {
            return StatusCode(500, "An unexpected error occured");
        }
    }

    //Everybody -> Guests shouldn't be able to see rooms, so not everybody
    /// <summary>
    /// Show one specific room
    /// </summary>
    /// <param name="id">Unique identifier for room</param>
    /// <returns>A room</returns>
    /// <response code="200">Room succsessfully found!</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Rooms not found!</response>
    /// <response code="500">Internal server errot</response>
    [Authorize(Roles = "Admin,Reception,CleaningStaff")]
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomsDto>> GetSpecificRoom(int id)
    {
        try
        {
            return Ok(await _roomService.GetRoomByID(id));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occured");
        }
    }
    /// <summary>
    /// Creates a new room
    /// </summary>
    /// <param name="createRoom">Identifier for the new room</param>
    /// <returns>The newly created room</returns>
    /// <response code="204">Succssesful!</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Room could not be created!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateRoom(RoomCreateDto createRoom)
    {
        try
        {
            await _roomService.PostRoom(createRoom);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexcpeted error occured");
        }
    }

    /// <summary>
    /// Gets all rooms with the specified roomtype id
    /// </summary>
    /// <param name="roomTypeId">Unique identifier for the roomtypes</param>
    /// <returns>A list of the rooms with the specified id</returns>
    /// <response code="200">Rooms with roomtype successfully found!</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Could not find rooms!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Receptionist")]
    [HttpGet("{roomTypeId}/Id-for-roomtypes")]
    public async Task<ActionResult<RoomType>> GetRoomsByRoomType(int roomTypeId)
    {
        try
        {
            var rooms = await _roomService.GetRoomsByRoomType(roomTypeId);
            return Ok(rooms);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexcpected error occured");
        }
        
    }
}