using DomainModels.Dto;
using API.Data;
using Microsoft.EntityFrameworkCore;
using DomainModels.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;

namespace API.Services
{
    public class HotelService
    {
        private readonly AppDBContext _context;
        public HotelService(AppDBContext context)
        {
            _context = context;
        }

        //GET
        public async Task<IEnumerable<HotelDto>> GetHotel()
        {

            var hotels = await _context.Hotels.ToListAsync();

            if (hotels == null)
            {
                return null;
            }

            var getHotels = hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                HotelName = h.HotelName,
                CityName = h.CityName,
                Address = h.Address,
                Description = h.Description,
            }).ToList();

            return getHotels;

        }
        
       
        //GET {Id}
        public async Task<HotelDto> GetHotelById(int id)
        {
            if (id == null)
            {
                return null;
            }

            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
            {
                return null;
            }

            HotelDto getHotel = new HotelDto
            {
                Id = hotel.Id,
                HotelName = hotel.HotelName,
                CityName = hotel.CityName,
                Address = hotel.Address,
                Description = hotel.Description,
            };

            return getHotel;
        }

        //Use type Hotel instead of HotelDto, as we want the new hotel into the DB
        public async Task<Hotel> PostHotel(HotelDto hotelCreateDto)
        {
            //Check if hotel.name already exists in our database
            if (await _context.Hotels.AnyAsync(h => h.HotelName == hotelCreateDto.HotelName))
            {
                return null;
            }
            var newHotel = new Hotel
            {
                HotelName = hotelCreateDto.HotelName,
                CityName = hotelCreateDto.CityName,
                Address = hotelCreateDto.Address,
                Description = hotelCreateDto.Description,
                CreatedAt = DateTime.UtcNow.AddHours(2),
                UpdatedAt = DateTime.UtcNow.AddHours(2)
            };
            _context.Hotels.Add(newHotel);

            await _context.SaveChangesAsync();

            //Return new Hotel into db
            return newHotel;
        }
        
        /// <summary>
        /// Returns HotelDto, as we want the "tailored" version
        /// </summary>
        public async Task<HotelDto?> PutHotel(HotelDto updatedHotel)
        {
            var currentHotel = await _context.Hotels.FindAsync(updatedHotel.Id);
            if (currentHotel == null)
            {
                return null;
            }

            currentHotel.Id = updatedHotel.Id;
            currentHotel.HotelName = updatedHotel.HotelName;
            currentHotel.CityName = updatedHotel.CityName;
            currentHotel.Address = updatedHotel.Address;
            currentHotel.Description = updatedHotel.Description;


            return new HotelDto
            {
                Id = currentHotel.Id,
                HotelName = currentHotel.HotelName,
                CityName = currentHotel.CityName,
                Address = currentHotel.Address,
                Description = currentHotel.Description,
            };
        }

        public async Task<bool> DeleteHotel(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
            {
                return false;
            }
            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}