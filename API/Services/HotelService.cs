using DomainModels.Dto;
using API.Data;
using Microsoft.EntityFrameworkCore;
using DomainModels.Models;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc;
using DomainModels.Mapping;

namespace API.Services
{
    public class HotelService
    {
        private readonly AppDBContext _context;
        private readonly HotelMapping _hotelMapping = new();
        /// <summary>
        /// Constructor for the HotelService class
        /// </summary>
        /// <param name="context">Database context</param>
        public HotelService(AppDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all the hotels fom the database
        /// </summary>
        /// <returns>The hotels</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<HotelDto>> GetHotel()
        {
            var hotels = await _context.Hotels.ToListAsync()
            ?? throw new ArgumentException("No hotels found");

            return hotels
            .Select(h => _hotelMapping.ToHotelGETdto(h))
            .ToList();
        }

        /// <summary>
        /// View one specific hotel by id
        /// </summary>
        /// <param name="id">unique identifier for the hotel</param>
        /// <returns>The specified hotel</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<HotelDto> GetHotelById(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id can't be 0");

            var hotel = await _context.Hotels.FindAsync(id)
                ?? throw new ArgumentException($"Couldn't find hotel with ID: {id}");

            return _hotelMapping.ToHotelGETdto(hotel);
        }
        
        /// <summary>
        /// Creates a new hotel
        /// </summary>
        /// <param name="hotelCreateDto">Details for the new hotel</param>
        /// <returns>The new created hotel</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<HotelDto> PostHotel(HotelDto hotelCreateDto)
        {
            //Check if hotel.name already exists in our database
            if (await _context.Hotels.AnyAsync(h => h.HotelName == hotelCreateDto.HotelName))
                throw new ArgumentException($"Hotel already exist: {hotelCreateDto.HotelName}");

            var newHotel = _hotelMapping.ToHotelPOSTDto(hotelCreateDto);

            _context.Hotels.Add(newHotel);
            await _context.SaveChangesAsync();


            return _hotelMapping.ToHotelGETdto(newHotel);
        }

        /// <summary>
        /// Updates an hotel
        /// </summary>
        /// <param name="updatedHotel">The details of hotel we want updated</param>
        /// <returns>The new updated hotel</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<HotelDto?> PutHotel(HotelDto updatedHotel)
        {
            var currentHotel = await _context.Hotels.FindAsync(updatedHotel.Id)
                ?? throw new ArgumentException($"Couldn't find hotel by ID: {updatedHotel.Id}");

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

        /// <summary>
        /// Deletes an hotel
        /// </summary>
        /// <param name="id">Id of the hotel we want deleted</param>
        /// <returns>True, if the hotel was deleted</returns>
        public async Task<bool> DeleteHotel(int id)
        {
            if (id == 0)
                throw new ArgumentException("Id can't be 0");

            var hotel = await _context.Hotels.FindAsync(id);
            if (hotel == null)
                throw new ArgumentException($"Cound't find hotel with the Id: {id}");

            _context.Hotels.Remove(hotel);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}