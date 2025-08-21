using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Services;
using DomainModels.Models;
using System.ComponentModel.DataAnnotations;




[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly HotelService _hotelService;

    public HotelController(AppDBContext context, HotelService hotelService)
    {
        _context = context;
        _hotelService = hotelService;
    }

    //Everybody (?)
    // GET: api/Hotels
    /// <summary>
    /// Shows all hotels
    /// </summary>
    /// <returns>A list of the hotels</returns>
    /// <response code="404"> Hotels not found! </response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
    {
        try
        {
            var hotels = await _hotelService.GetHotel();

            if (hotels == null)
            {
                return BadRequest("Cannot find hotel");
            }

            return Ok(hotels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error {ex.Message}");
        }
        
    }

    /// <summary>
    /// Shows one specific hotel
    /// </summary>
    /// <param name="id">Unique identifier for hotels</param>
    /// <returns>A hotel</returns>
    /// <response code="404"> Hotels not found! </response>
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetSpecificHotel(int id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var hotel = _hotelService.GetHotelById(id);
        return Ok(hotel);
    }

    //Only Admin
    // POST: api/Hotels
    /// <summary>
    /// Creates a new hotel
    /// </summary>
    /// <param name="hotelcreateDto">Contains hotel details to be created</param>
    /// <returns>The newly created hotel</returns>
    /// <response code="400">Could not create hotel!</response>
    [HttpPost]
    public async Task<ActionResult> CreateHotel(HotelDto hotelcreateDto)
    {
        
        try
        {
            var newHotel = await _hotelService.PostHotel(hotelcreateDto);
            

            if (newHotel == null)
            {
                return BadRequest();
            }

            return Ok(newHotel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }

    }

    //Only Admin
    //PUT: api/Hotels
    /// <summary>
    /// Updates a specific hotel
    /// </summary>
    /// <param name="updateHotel">Contains hotel details to be updated</param>
    /// <returns>Updated hotel</returns>
    /// <response code="400">Could not update hotel!</response>
    [HttpPut]
    public async Task<ActionResult> PutHotel(HotelDto updateHotel)
    {
        try
        {
            var _updatedHotel = await _hotelService.PutHotel(updateHotel);
            await _context.SaveChangesAsync();

            return Ok(_updatedHotel);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
        
    }

    //Only Admin
    //DELETE: api/Hotels
    /// <summary>
    /// Deletes a hotel
    /// </summary>
    /// <param name="Id">Unique identifier</param>
    /// <returns>true if deletion succeed</returns>
    /// <response code="404">Could not delete hotel!</response>
    [HttpDelete]
    public async Task<IActionResult> DeleteHotel(int Id)
    {
        var deletedHotel = await _hotelService.DeleteHotel(Id);

        return Ok(deletedHotel);
    }

}
