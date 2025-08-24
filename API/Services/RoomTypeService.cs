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
            this._context = context;
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
            };

            return getRoomType;
        }

        public async Task<RoomType> UpdateRoomType(RoomTypePutDto roomTypePutDto)
        {
            var existingRoomType = await _context.RoomTypes.FindAsync(roomTypePutDto.Id);
            if (existingRoomType == null)
            {
                return null;
            }

            existingRoomType.Id = roomTypePutDto.Id;
            existingRoomType.Description = roomTypePutDto.Description;
            existingRoomType.PricePerNight = roomTypePutDto.Price;
            existingRoomType.UpdatedAt = roomTypePutDto.UpdatedAt;

            await _context.SaveChangesAsync();

            return existingRoomType;
        }

        // var type = _context.RoomTypes.Find((int)roomTypeDto.TypeofRoom);
        //     if (type == null)
        //     {
        //         return null;
        //     }

        // public async Task<RoomType> UpdateRoomTypes(RoomTypeDto roomTypeDto)
        // {

        // }

        // public async Task<RoomType> PostRoomType(RoomTypeDto roomTypeDto)
        // {
        //     if (await _context.RoomTypes.AnyAsync(rt => rt.Id == roomTypeDto.Id))
        //     {
        //         return null;
        //     }

        //     

        //     var newRoomType = new RoomType
        //     {
        //         Id = roomTypeDto.Id,
        //         TypeofRoom = type.TypeofRoom,
        //         MaxCapacity = roomTypeDto.MaxCapacity,
        //         Description = roomTypeDto.Description,
        //     };

        //     _context.RoomTypes.Add(newRoomType);
        //     await _context.SaveChangesAsync();

        //     return newRoomType;
        // }

    }
}
   