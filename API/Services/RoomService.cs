using API.Data;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomService
    {
        private readonly AppDBContext _context;
        public RoomService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomsDto>> GetRooms()
        {
            var rooms = await _context.Rooms.Include(r => r.RoomType).Include(r => r.Hotel).ToListAsync();
            return rooms.Select(r => new RoomsDto
            {
                Id = r.Id,
                //GuestCount = r.GuestCount,
                IsAvailable = r.IsAvailable,
                IsBreakfast = r.IsBreakfast,
                AvailableFrom = r.AvailableFrom,
                RoomTypeId = r.RoomType.TypeofRoom,
                HotelId = r.Hotel.Id,

            });
        }

        public async Task<RoomsDto> GetRoomByID(int id)
        {
            if (id == null)
            {
                return null;
            }

            var room = await _context.Rooms.FindAsync(id);

            RoomsDto getRoom = new RoomsDto
            {
                Id = room.Id,
                //GuestCount = room.GuestCount,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                AvailableFrom = room.AvailableFrom,
                RoomTypeId = room.RoomType.TypeofRoom,
            };

            return getRoom;
        }
        
        //Use type Room instead of RoomsDto, as we want the new room into the DB
        public async Task<Room> PostRoom(RoomsDto room)
        {
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber))
            {
                return null;
            }
            var type = _context.RoomTypes.Find((int)room.RoomTypeId);
            if (type == null)
            {
                return null;
            }

            var newRoom = new Room
            {
                Id = room.Id,
                //GuestCount = room.GuestCount,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                RoomNumber = room.RoomNumber,
                TypeId = type.Id,
                HotelId = room.HotelId,
            };
            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return newRoom;
        }

    }
}