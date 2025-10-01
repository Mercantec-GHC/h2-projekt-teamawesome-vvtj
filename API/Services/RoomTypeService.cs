using API.Data;
using API.Helpers;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DomainModels.Enums;
using DomainModels.Mapping;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomTypeService
    {
        private readonly AppDBContext _context;
        private readonly RoomTypeMapping _mapping = new();
        public RoomTypeService(AppDBContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<RoomTypeDto?>> GetRoomTypes()
        {
            var roomtypes = await _context.RoomTypes.ToListAsync()
            ?? throw new ArgumentException("No roomtypes found");

            return roomtypes
              .Select(rt => _mapping.ToRoomTypeGETdto(rt));
        }

        public async Task<RoomTypeDto?> GetSpecificRoomType(int roomtypeId)
        {
            if (roomtypeId == 0)
                throw new ArgumentException("ID cannot be 0");

            var roomtype = await _context.RoomTypes.FindAsync(roomtypeId)
            ?? throw new ArgumentException($"No roomtype was found with the given ID: {roomtypeId}");
            
            return _mapping.ToRoomTypeGETdto(roomtype);
        }
        
        /// <summary>
        /// Returns RoomType (Model), as we want to see the entire
        /// roomtype model, with the updated fields
        /// </summary>
        public async Task<RoomType?> UpdateRoomType(int roomtypeId, RoomTypePutDto roomTypePutDto)
        {
            if (roomtypeId == 0)
                throw new ArgumentException("ID cannot be 0");

            var existingRoomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomtypeId)
            ?? throw new ArgumentException($"Could not find room type with ID:{roomtypeId}");

            //Only update if there's a new value
            if (!string.IsNullOrWhiteSpace(roomTypePutDto.Description) && roomTypePutDto.Description != "string")
                existingRoomType.Description = roomTypePutDto.Description;

            //Only update if there's a new value
            if (roomTypePutDto.Price > 0)
                existingRoomType.PricePerNight = roomTypePutDto.Price;


            existingRoomType.UpdatedAt = DateTime.UtcNow.AddHours(2);

            await _context.SaveChangesAsync();
            return existingRoomType;
        }

    }
}
   