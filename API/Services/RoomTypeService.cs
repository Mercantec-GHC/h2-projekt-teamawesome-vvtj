using DomainModels.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomTypeService
    {
        private readonly AppDBContext context;
        public RoomTypeService(AppDBContext _context)
        {
            context = _context;
        }
        public async Task<IEnumerable<RoomTypeDto>> GetRoomTypes()
        {
            var roomtypes = await context.RoomTypes.ToListAsync();

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
            var roomtype = await context.RoomTypes.FindAsync(roomtypeId);

            RoomTypeDto getRoomType = new RoomTypeDto
            {
                Id = roomtype.Id,
                TypeofRoom = roomtype.TypeofRoom,
                MaxCapacity = roomtype.MaxCapacity,
                Description = roomtype.Description
            };

            return getRoomType;
        }

    }
}
