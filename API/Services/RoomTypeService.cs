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
            var roomtypes = await _context.RoomTypes.ToListAsync();

            if (roomtypes == null)
            {
                return null;
            }

            var newRoomTypesListDto = roomtypes
              .Select(rt => _mapping.ToRoomTypeGETdto(rt));

            // if (!RoomTypeEnumHelper.TryToConvert(rt.TypeofRoom, out var roomTypeEnum))
            //     return null;
            return newRoomTypesListDto;
        }

        public async Task<RoomTypeDto?> GetSpecificRoomType(int roomtypeId)
        {
            var roomtype = await _context.RoomTypes.FindAsync(roomtypeId);
            if (roomtype == null)
            {
                return null;
            }
            //  if (!RoomTypeEnumHelper.TryToConvert(, out var roomTypeEnum))
            //  {
            //      return null;
            //  }


            var getRoomType = _mapping.ToRoomTypeGETdto(roomtype);
            return getRoomType;
        }

        /// <summary>
        /// Returns RoomType (Model), as we want to see the entire
        /// roomtype model, with the updated fields
        /// </summary>
        public async Task<RoomType?> UpdateRoomType(int roomtypeId, RoomTypePutDto roomTypePutDto)
        {
            var existingRoomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomtypeId);
            if (existingRoomType == null)
            {
                return null;
            }

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
   