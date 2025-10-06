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
using Org.BouncyCastle.Asn1.Esf;

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
        /// <summary>
        /// Retrieves all the roomtypes from the database
        /// </summary>
        /// <returns>Lists of all the roomtypes</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<RoomTypeDto?>> GetRoomTypes()
        {
            var roomtypes = await _context.RoomTypes.ToListAsync()
            ?? throw new ArgumentException("No roomtypes found");

            return roomtypes
              .Select(rt => _mapping.ToRoomTypeGETdto(rt));
        }
        
        /// <summary>
        /// View a single roomtype with unique id
        /// </summary>
        /// <param name="roomtypeId">The roomtype to view</param>
        /// <returns>The chosen roomtype</returns>
        /// <exception cref="ArgumentException"></exception>
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
       /// <param name="roomtypeId"> The roomtype we want updated</param>
       /// <param name="roomTypePutDto">What we want updated</param>
       /// <returns>Updated roomtype</returns>
       /// <exception cref="ArgumentException"></exception>
        public async Task<RoomTypePutDto?> UpdateRoomType(int roomtypeId, RoomTypePutDto roomTypePutDto)
        {
            if (roomtypeId == 0)
                throw new ArgumentException("ID cannot be 0");

            var existingRoomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomtypeId)
            ?? throw new ArgumentException($"Could not find room type with ID:{roomtypeId}");

            _mapping.ApplyRoomTypePutdto(roomTypePutDto, existingRoomType);
            await _context.SaveChangesAsync();

            return _mapping.ToRoomTypePUTdto(existingRoomType);
        }
    }
}
   