using API.Data;
using DomainModels.Mapping;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class RoomService
    {
        private readonly AppDBContext _context;
        private readonly RoomMapping _roomMapping = new();
        public RoomService(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RoomsDto>> GetRooms()
        {
            var rooms = await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel).ToListAsync();

            return rooms
            .Select(r => _roomMapping.ToRoomGETdto(r))
            .ToList();
        }

        public async Task<RoomsDto> GetRoomByID(int id)
        {
            //if statement and ??
            if (id == 0)
                throw new ArgumentException("id cant be 0");

            var room = await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new ArgumentException($"Couldnt find room with id; {id}");

            return _roomMapping.ToRoomGETdto(room);
        }

        //Use type Room instead of RoomsDto, as we want the new room into the DB
        public async Task<Room> PostRoom(RoomCreateDto room)
        {
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber))
            {
                Console.WriteLine("Room already exists");
                return null;
            }

            var type = _context.RoomTypes.Find(room.RoomtypeId);
            if (type == null)
            {
                Console.WriteLine("RoomType doesn't exist");
                return null;
            }

            var newRoom = new Room
            {
                Id = room.Id,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                RoomNumber = room.RoomNumber,
                TypeId = type.Id,
                HotelId = room.HotelId,
            };
            _context.Rooms.Add(newRoom);
            await _context.SaveChangesAsync();
            Console.WriteLine("Added");

            return newRoom;
        }
        public async Task<IEnumerable<RoomsDto>> GetRoomsByRoomType(int roomTypeId)
        {
            var rooms = await _context.Rooms
            .Where(r => r.TypeId == roomTypeId)
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .ToListAsync();

            return rooms
            .Select(r => _roomMapping.ToRoomGETdto(r))
            .ToList();
        }
    }
}