using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Data;
using DomainModels.Models;




[ApiController]
[Route("api/[controller]")]
public class HotelController : ControllerBase
{
    private readonly AppDBContext Context;


    public HotelController(AppDBContext context)
    {
        Context = context;
    }

    //Everybody (?)
    // GET: api/Hotels
    [HttpGet]
    public async Task<ActionResult<IEnumerable<HotelViewDto>>> GetHotels()
    {

        var hotels = await Context.Hotels.ToListAsync();

        if (hotels == null)
        {
            return BadRequest("Cannot find hotel");
        }

        var newHotelListGetDtos = hotels.Select(h => new HotelViewDto
        {
            Id = h.Id,
            HotelName = h.HotelName,
            CityName = h.CityName,
            Description = h.Description,
        }).ToList();
        return Ok(newHotelListGetDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HotelViewDto>> GetSpecificHotel(int id)
    {
        var hotel = await Context.Hotels.FindAsync(id);

        HotelViewDto getHotel = new HotelViewDto
        {
            Id = hotel.Id,
            HotelName = hotel.HotelName,
            CityName = hotel.CityName,
            Description = hotel.Description,
        };

        return Ok(getHotel);
    }

    //Only Admin
    // POST: api/Hotels
    [HttpPost]
    public async Task<ActionResult> CreateHotel(HotelCreateDto hotelcreateDto)
    {
        Hotel hotel = new Hotel
        {
            HotelName = hotelcreateDto.HotelName,
            CityName = hotelcreateDto.CityName,
            Address = hotelcreateDto.Address,
            Description = hotelcreateDto.Description,
            CreatedAt = DateTime.UtcNow.AddHours(2),
            UpdatedAt = DateTime.UtcNow.AddHours(2),
        };

        Context.Hotels.Add(hotel);
        await Context.SaveChangesAsync();

        return Created();
    }

    //Only Admin
    //PUT: api/Hotels
    [HttpPut]
    public async Task<ActionResult> PutHotel(HotelUpdateDto updateHotel)
    {
        Context.Entry(updateHotel).State = EntityState.Modified;
        await Context.SaveChangesAsync();

        return Created();
    }

    //Only Admin
    //DELETE: api/Hotels
    [HttpDelete]
    public async Task<IActionResult> DeleteHotel(int Id)
    {
        var hotel = await Context.Hotels.FindAsync(Id);

        if (hotel == null)
        {
            NotFound("Could not find hotel");
        }

        Context.Hotels.Remove(hotel);
        await Context.SaveChangesAsync();

        return NoContent();
    }

}
