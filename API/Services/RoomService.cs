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
                RoomType = r.RoomType,
                RoomTypeName = r.RoomType.TypeofRoom.ToString(),
                HotelId = r.Hotel.Id,
                HotelName = r.Hotel.HotelName,
                RoomNumber = r.RoomNumber,

            });
        }

        public async Task<RoomsDto> GetRoomByID(int id)
        {
            if (id == null)
            {
                return null;
            }

            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return null;
            }
            Console.WriteLine(room.Id);
            Console.WriteLine(room.IsAvailable);


            var getRoom = new RoomsDto
            {
                Id = room.Id,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                AvailableFrom = room.AvailableFrom,
                RoomType = room.RoomType,
                RoomTypeName = room.RoomType.TypeofRoom.ToString(),
                HotelName = room.Hotel.HotelName
            };


            Console.WriteLine("New Room ID" + getRoom.Id);
            Console.WriteLine(getRoom.RoomType);
            return getRoom;
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

            var roomsWithType = rooms.Select(r => new RoomsDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType,
                RoomTypeName = r.RoomType.TypeofRoom.ToString(),
                HotelName = r.Hotel.HotelName,
                HotelId = r.HotelId,

            });

            return roomsWithType;

        }

    }
}