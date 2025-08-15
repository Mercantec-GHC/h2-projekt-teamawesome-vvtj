using DomainModels.Models;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

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

    //Everybody
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

    //Everybody
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

}