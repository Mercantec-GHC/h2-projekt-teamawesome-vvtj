using DomainModels.Models;
using API.Data;
using API.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DomainModels.Enums;

namespace API.Services
{
    public class RoomTypeService
    {
        private readonly AppDBContext _context;
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
            .Select(rt =>
            {
                if (!RoomTypeEnumHelper.TryToConvert(rt.TypeofRoom, out var roomTypeEnum))
                    return null;
                return new RoomTypeDto
                {
                    Id = rt.Id,
                    TypeofRoom = roomTypeEnum.ToString(),
                    MaxCapacity = rt.MaxCapacity,
                    Description = rt.Description,
                    HasBalcony = rt.HasBalcony,
                    HasJacuzzi = rt.HasJacuzzi,
                    HasKitchenette = rt.HasKitchenette,
                    HasExtraTowels = rt.HasExtraTowels,
                    HasAirCondition = rt.HasAirCondition,
                    HasGardenView = rt.HasGardenView,
                    HasKettle = rt.HasKettle,
                    HasMiniFridge = rt.HasMiniFridge,
                    HasSeaView = rt.HasSeaView,
                    HasTV = rt.HasTV,
                    HasVault = rt.HasVault,
                    Area = rt.Area,
                    PricePerNight = rt.PricePerNight,
                };
            }).ToList();
            return newRoomTypesListDto;
        }

        public async Task<RoomTypeDto> GetSpecificRoomType(int roomtypeId)
        {
            var roomtype = await _context.RoomTypes.FindAsync(roomtypeId);
            if (!RoomTypeEnumHelper.TryToConvert(roomtype.TypeofRoom, out var roomTypeEnum))
            {
                return null;
            }

            RoomTypeDto getRoomType = new RoomTypeDto
            {
                Id = roomtype.Id,
                TypeofRoom = roomTypeEnum.ToString(),
                MaxCapacity = roomtype.MaxCapacity,
                Description = roomtype.Description,
                HasBalcony = roomtype.HasBalcony,
                HasJacuzzi = roomtype.HasJacuzzi,
                HasKitchenette = roomtype.HasKitchenette,
                HasExtraTowels = roomtype.HasExtraTowels,
                HasAirCondition = roomtype.HasAirCondition,
                HasGardenView = roomtype.HasGardenView,
                HasKettle = roomtype.HasKettle,
                HasMiniFridge = roomtype.HasMiniFridge,
                HasSeaView = roomtype.HasSeaView,
                HasTV = roomtype.HasTV,
                HasVault = roomtype.HasVault,
                Area = roomtype.Area,
                PricePerNight = roomtype.PricePerNight,
            };

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
            if (!string.IsNullOrWhiteSpace(roomTypePutDto.Description))
                existingRoomType.Description = roomTypePutDto.Description;


            //Only update if there's a new value
            if (roomTypePutDto.Price > 0)
            if (roomTypePutDto.Price > 0)
                existingRoomType.PricePerNight = roomTypePutDto.Price;


            existingRoomType.UpdatedAt = DateTime.UtcNow.AddHours(2);


            existingRoomType.UpdatedAt = roomTypePutDto.UpdatedAt;

            await _context.SaveChangesAsync();
            return existingRoomType;
        }

    }
}
   