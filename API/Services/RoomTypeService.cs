using DomainModels.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomTypeService
    {
        private readonly AppDBContext _context;
        public RoomTypeService(AppDBContext context)
        {
            this._context = context;
        }
        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypes()
        {
            var roomtypes = await _context.RoomTypes.ToListAsync();

            if (roomtypes == null)
            {
                return null;
            }

            var newRoomTypesListDto = roomtypes.Select(rt => new RoomTypeDto
            {
                Id = rt.Id,
                TypeofRoom = rt.TypeofRoom,
                MaxCapacity = rt.MaxCapacity,
                Description = rt.Description
            }).ToList();
            return newRoomTypesListDto;
        }

        public async Task<RoomTypeDto> GetSpecificRoomType(int roomtypeId)
        {
            var roomtype = await _context.RoomTypes.FindAsync(roomtypeId);

            RoomTypeDto getRoomType = new RoomTypeDto
            {
                Id = roomtype.Id,
                TypeofRoom = roomtype.TypeofRoom,
                MaxCapacity = roomtype.MaxCapacity,
                Description = roomtype.Description
            };

            return getRoomType;
        }

        public async Task<RoomType> PostRoomType(RoomTypeDto roomTypeDto)
        {
            if (await _context.RoomTypes.AnyAsync(rt => rt.Id == roomTypeDto.Id))
            {
                return null;
            }

            var newRoomType = new RoomType
            {
                Id = roomTypeDto.Id,
                TypeofRoom = roomTypeDto.TypeofRoom,
                MaxCapacity = roomTypeDto.MaxCapacity,
                Description = roomTypeDto.Description
            };
            _context.RoomTypes.Add(newRoomType);
            await _context.SaveChangesAsync();

            return newRoomType;
        }

    }
}
