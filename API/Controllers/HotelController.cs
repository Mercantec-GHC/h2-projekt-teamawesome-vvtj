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
    /// <response code="404"> Hotels not found! </response>
    [HttpGet("{id}")]
    public async Task<ActionResult<HotelDto>> GetSpecificHotel(int id)
    {
        try
        {
            return await _hotelService.GetHotelById(id);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    //Only Admin
    // POST: api/Hotels
    /// <summary>
    /// Creates a new hotel
    /// </summary>
    /// <param name="hotelcreateDto">Contains hotel details to be created</param>
    /// <returns>The newly created hotel</returns>
    /// <response code="400">Could not create hotel!</response>
    /// 
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
    }

	//Only Admin
	//PUT: api/Hotels
	/// <summary>
	/// Updates a specific hotel
	/// </summary>
	/// <param name="updateHotel">Contains hotel details to be updated</param>
	/// <returns>Updated hotel</returns>
	/// <response code="400">Could not update hotel!</response>
	/// 
	[Authorize(Roles = "Admin")]
	[HttpPut]
    public async Task<ActionResult> PutHotel(HotelDto updateHotel)
    {
        try
        {
            var _updatedHotel = await _hotelService.PutHotel(updateHotel);
            await _context.SaveChangesAsync();

            return Ok(_updatedHotel);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        
    }
    
    [HttpPatch("{hotelId}")]
    public async Task<IActionResult> PatchHotel(int hotelId, [FromBody]HotelPutDto updateDto)
    {
        var hotel = await _context.Hotels.FindAsync(hotelId);
        if (hotel == null) return NotFound();

        if (updateDto.HotelName != null) hotel.HotelName = updateDto.HotelName;
        if (updateDto.CityName != null) hotel.CityName = updateDto.CityName;
        if (updateDto.Address != null) hotel.Address = updateDto.Address;
        if (updateDto.Description != null) hotel.Description = updateDto.Description;

        hotel.UpdatedAt = DateTime.UtcNow.AddHours(2);
        await _context.SaveChangesAsync();

        return Ok(hotel);
        // var exitingHotel = await _context.Hotels.FindAsync(hotelId);

        // var updatedHotel = new HotelPutDto
        // {
        //     HotelName = exitingHotel.HotelName,
        //     CityName = exitingHotel.CityName,
        //     Address = exitingHotel.Address,
        //     Description = exitingHotel.Description,
        // };
        // patchDocumentHotel.ApplyTo(updatedHotel, ModelState);

        // _hotelMapping.ToHotelPatchDto(exitingHotel, updatedHotel);

        // await _context.SaveChangesAsync();
        // return Ok(exitingHotel);
    }

    //Only Admin
    //DELETE: api/Hotels
    /// <summary>
    /// Deletes a hotel
    /// </summary>
    /// <param name="Id">Unique identifier</param>
    /// <returns>true if deletion succeed</returns>
    /// <response code="404">Could not delete hotel!</response>
    /// 
    [Authorize(Roles = "Admin")]
	[HttpDelete]
    public async Task<IActionResult> DeleteHotel(int Id)
    {
        var deletedHotel = await _hotelService.DeleteHotel(Id);

        return Ok(deletedHotel);
    }

}
