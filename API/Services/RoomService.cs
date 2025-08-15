using API.Data;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomService
    {
        private readonly AppDBContext context;
        public RoomService(AppDBContext _context)
        {
            context = _context;
        }

        public async Task<IEnumerable<RoomsDto>> GetRooms()
        {
            var rooms = await context.Rooms.Include(r => r.RoomType).ToListAsync();
            return rooms.Select(r => new RoomsDto
            {
                Id = r.Id,
                GuestCount = r.GuestCount,
                IsAvailable = r.IsAvailable,
                IsBreakfast = r.IsBreakfast,
                AvailableFrom = r.AvailableFrom,
                RoomType = r.RoomType,
                TypeId = r.TypeId
            });
        }

        public async Task<RoomsDto> GetRoomByID(int id)
        {
            if (id == null)
            {
                return null;
            }

            var room = await context.Rooms.FindAsync(id);

            RoomsDto getRoom = new RoomsDto
            {
                Id = room.Id,
                GuestCount = room.GuestCount,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                AvailableFrom = room.AvailableFrom,
                RoomType = room.RoomType,
            };

            return getRoom;
        }

    }
}