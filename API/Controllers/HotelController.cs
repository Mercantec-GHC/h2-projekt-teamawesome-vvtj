using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using API.Services;
using DomainModels.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc.NewtonsoftJson;
using Azure;
using DomainModels.Mapping;


[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly AppDBContext _context;
    private readonly HotelService _hotelService;
    private readonly HotelMapping _hotelMapping;

    public HotelController(AppDBContext context, HotelService hotelService)
    {
        _context = context;
        _hotelService = hotelService;
    }


    // GET: api/Hotels
    /// <summary>
    /// Shows all hotels
    /// </summary>
    /// <returns>A list of the hotels</returns>
    /// <response code="200">Hotels successfully found</response>
    /// <response code="404"> Hotels not found! </response>
    /// <response code="500">Internal server error</response>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelDto>>> GetHotels()
    {
        try
        {
            return Ok(await _hotelService.GetHotel());
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Shows one specific hotel
    /// </summary>
    /// <param name="id">Unique identifier for hotels</param>
    /// <returns>A hotel</returns>
    /// <response code="200">Hotel successfully found!</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404"> Hotel not found! </response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetSpecificHotel(int id)
    {
        try
        {
            return Ok(await _hotelService.GetHotelById(id));
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

    //Only Admin
    // POST: api/Hotels
    /// <summary>
    /// Creates a new hotel
    /// </summary>
    /// <param name="hotelcreateDto">Contains hotel details to be created</param>
    /// <returns>The newly created hotel</returns>
    /// <response code="204">Successful"</response>
    /// <response code="400">Could not create hotel!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult> CreateHotel(HotelDto hotelcreateDto)
    {
        try
        {
            await _hotelService.PostHotel(hotelcreateDto);
            return NoContent();
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

    //Only Admin
    //PUT: api/Hotels
    /// <summary>
    /// Updates a specific hotel
    /// </summary>
    /// <param name="id">Identifier for the hotel we want to update</param>
    /// <param name="updateHotel">Contains hotel details to be updated</param>
    /// <returns>Updated hotel</returns>
    /// <response code="200">Hotel succssesfully updated!</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Could not find hotel!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin")]
    [HttpPut]
    public async Task<ActionResult> PutHotel(int id, HotelDto updateHotel)
    {
        try
        {
            return Ok(await _hotelService.PutHotel(id, updateHotel));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception)
        {
            return StatusCode(500, "An unexpected error occurd");
        }
    }


    //Only Admin
    //DELETE: api/Hotels
    /// <summary>
    /// Deletes a hotel
    /// </summary>
    /// <param name="Id">Unique identifier</param>
    /// <returns>true if deletion succeed</returns>
    /// <response code="200">Hotel succssefullt deleted</response>
    /// <response code="400">Invalid input</response>
    /// <response code="404">Could not find hotel!</response>
    /// <response code="500">Internal server error</response>
    [Authorize(Roles = "Admin")]
    [HttpDelete]
    public async Task<IActionResult> DeleteHotel(int Id)
    {
        try
        {
            var deletedHotel = await _hotelService.DeleteHotel(Id);

            return Ok(deletedHotel);
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
