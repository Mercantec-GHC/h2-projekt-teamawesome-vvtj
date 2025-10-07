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
    /// <response code="200">Roomtypes successfully found!</response>
    /// <response code="404">Room types not found!</response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetRoomTypes()
    {
        try
        {
            return Ok(await _roomtypeService.GetRoomTypes());
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexcepted error occured");
        }
    }

    //Everybody
    /// <summary>
    /// Show one specific room type
    /// </summary>
    /// <param name="id">Unique identifier for room types</param>
    /// <returns>A room type</returns>
    /// <response code="200">Roomtype successfully found!</response>
    /// <response code="400">Invalid input error</response>
    /// <response code="404">Room type not found!</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<RoomTypeDto>> GetSpecificRoomtype(int id)
    {
        try
        {
            return Ok(await _roomtypeService.GetSpecificRoomType(id));
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexcepted error occured");
        }
    }

    /// <summary>
    /// Updates either price or description for a roomtype
    /// </summary>
    /// <param name="id">Unique identifier for room types</param>
    /// <param name="roomTypePutDto">The updated roomtype data</param>
    /// <returns>Updated roomtype</returns>
    /// <response code="200">Room type successfully updated.</response>
    /// <response code="400">Invalid input error</response>
    /// <response code="404">Room type not found.</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateRoomType(int id, RoomTypePutDto roomTypePutDto)
    {
        try
        {
            return Ok(await _roomtypeService.UpdateRoomType(id, roomTypePutDto));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"An unexpected error occured");
        }
    }
}

    