using DomainModels.Dto;
using API.Data;
using Microsoft.EntityFrameworkCore;
using DomainModels.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Data;
using DomainModels.Mapping;

namespace API.Services
{
    public class HotelService
    {
        private readonly AppDBContext _context;
        private readonly HotelMapping _hotelMapping;
        public HotelService(AppDBContext context)
        {
            _context = context;
        }

        //GET
        public async Task<IEnumerable<HotelDto>> GetHotel()
        {
            var hotels = await _context.Hotels.ToListAsync()
                ?? throw new ArgumentException("No hotels found");

            return hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                HotelName = h.HotelName,
                CityName = h.CityName,
                Address = h.Address,
                Description = h.Description,
                Phone = h.Phone,
                Email = h.Email,
                WeekdayTime = h.WeekdayTime,
                SaturdayTime = h.SaturdayTime,
                HolidaysTime = h.HolidaysTime
            }).ToList();
        }

        //GET {Id}
        public async Task<HotelDto> GetHotelById(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id can't be 0");

            var hotel = await _context.Hotels.FindAsync(id)
                ?? throw new ArgumentException($"Couldn't find hotel with ID: {id}");

            return new HotelDto
            {
                Id = hotel.Id,
                HotelName = hotel.HotelName,
                CityName = hotel.CityName,
                Address = hotel.Address,
                Description = hotel.Description,
                Email = hotel.Email,
                Phone = hotel.Phone,
                WeekdayTime = hotel.WeekdayTime,
                SaturdayTime = hotel.SaturdayTime,
                HolidaysTime = hotel.HolidaysTime
            };
        }

        //Use type Hotel instead of HotelDto, as we want the new hotel into the DB
        public async Task<Hotel> PostHotel(HotelDto hotelCreateDto)
        {
            //Check if hotel.name already exists in our database
            if (await _context.Hotels.AnyAsync(h => h.HotelName == hotelCreateDto.HotelName))
                throw new ArgumentException($"Hotel already exist: {hotelCreateDto.HotelName}");
            
            var newHotel = new Hotel
            {
                HotelName = hotelCreateDto.HotelName,
                CityName = hotelCreateDto.CityName,
                Address = hotelCreateDto.Address,
                Description = hotelCreateDto.Description,
                CreatedAt = DateTime.UtcNow.AddHours(2),
                UpdatedAt = DateTime.UtcNow.AddHours(2),
                Email = hotelCreateDto.Email,
                Phone = hotelCreateDto.Phone,
                WeekdayTime = hotelCreateDto.WeekdayTime,
                SaturdayTime = hotelCreateDto.SaturdayTime,
                HolidaysTime = hotelCreateDto.HolidaysTime
            };

            _context.Hotels.Add(newHotel);
            await _context.SaveChangesAsync();

            Console.WriteLine(newHotel.Id);
            
            var createdHotel = _context.Hotels.FirstOrDefault(h => h.HotelName == newHotel.HotelName)
                ?? throw new Exception("Something went wrong saving the hotel to the database!");

            Console.WriteLine(createdHotel.Id);

            return createdHotel;  
        }
        
        /// <summary>
        /// Returns HotelDto, as we want the "tailored" version
        /// </summary>
        public async Task<HotelDto?> PutHotel(HotelDto updatedHotel)
        {
            var currentHotel = await _context.Hotels.FindAsync(updatedHotel.Id)
                ?? throw new ArgumentException($"Couldn't not find hotel by ID:{updatedHotel.Id}");

            _hotelMapping.TohotelPUTDto(currentHotel, updatedHotel);

            await _context.SaveChangesAsync();

            return new HotelDto
            {
                Id = currentHotel.Id,
                HotelName = currentHotel.HotelName,
                CityName = currentHotel.CityName,
                Address = currentHotel.Address,
                Description = currentHotel.Description,
                Email = currentHotel.Email,
                Phone = currentHotel.Phone,
                WeekdayTime = currentHotel.WeekdayTime,
                SaturdayTime = currentHotel.SaturdayTime,
                HolidaysTime = currentHotel.HolidaysTime
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