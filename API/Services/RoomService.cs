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
        /// <summary>
        /// Retrieves all rooms
        /// </summary>
        /// <returns>All the rooms</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<RoomsDto>> GetRooms()
        {
            var rooms = await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel).ToListAsync()
            ?? throw new ArgumentException("No rooms found");

            return rooms
            .Select(r => _roomMapping.ToRoomGETdto(r))
            .ToList();
        }

        /// <summary>
        /// Shows one room
        /// </summary>
        /// <param name="id">Unique identifier for the specific room</param>
        /// <returns>The singular room</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<RoomsDto> GetRoomByID(int id)
        {
            if (id == 0)
                throw new ArgumentException("id cant be 0");

            var room = await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.Id == id)
                ?? throw new ArgumentException($"Couldnt find room with id: {id}");

            return _roomMapping.ToRoomGETdto(room);
        }
        
        /// <summary>
        /// Creates a room
        /// </summary>
        /// <param name="room">Details for the new room</param>
        /// <returns>The new created room</returns>
        /// <exception cref="ArgumentException"></exception>
        //Use type Room instead of RoomsDto, as we want the new room into the DB
        public async Task<Room> PostRoom(RoomCreateDto room)
        {
            if (await _context.Rooms.AnyAsync(r => r.RoomNumber == room.RoomNumber))
                throw new ArgumentException("Room already exists");
            

            var type = _context.RoomTypes.Find(room.RoomtypeId);
            if (type == null)
                throw new ArgumentException($"Roomtype with id: {type} does not exist");

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
        /// <summary>
        /// Shows all the rooms with the specific roomtype
        /// </summary>
        /// <param name="roomTypeId">Identifier for the roomtype</param>
        /// <returns>All the rooms with said roomtype</returns>
        /// <exception cref="ArgumentException"></exception>
        public async Task<IEnumerable<RoomsDto>> GetRoomsByRoomType(int roomTypeId)
        {
            if (roomTypeId == 0)
                throw new ArgumentException("Roomtype id cannot be 0");

            var rooms = await _context.Rooms
            .Where(r => r.TypeId == roomTypeId)
            .Include(r => r.RoomType)
            .Include(r => r.Hotel)
            .ToListAsync()
                ?? throw new ArgumentException($"No rooms found with the given roomtype {roomTypeId}");

            return rooms
            .Select(r => _roomMapping.ToRoomGETdto(r))
            .ToList();
        }
    }
}