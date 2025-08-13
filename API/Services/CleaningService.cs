using API.Interfaces;
using Dapper;
using DomainModels;
using DomainModels.Dto;
using Npgsql;

namespace API.Services
{
    public class CleaningService : ICleaningService
    {
        private readonly string _connectionString;

        public CleaningService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<RoomToCleanDto>> GetAllRoomsToCleanAsync()
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync(); 

            IEnumerable<Room> rooms = await GetRooms(connection);
            IEnumerable<Booking> booking = await GetBookings(connection);

            var roomsToClean = rooms
                .Where(r =>
                    !r.LastCleaned.HasValue || r.LastCleaned.Value <= DateTime.Now.AddDays(-3) ||
                    booking.Any(b =>
                        b.RoomId == r.Id && b.CheckOut <= DateTime.UtcNow))
                .ToList();

            var roomToCleanDtos = roomsToClean.Select(r => new RoomToCleanDto { RoomNumber = r.RoomNumber, RoomType = r.TypeId.ToString(), HotelName = r.HotelId.ToString()});

            return roomToCleanDtos;
        }

        private async Task<IEnumerable<Room>> GetRooms(NpgsqlConnection connection)
        {
            const string sql = @"SELECT * FROM ""Rooms""";
            var rooms = await connection.QueryAsync<Room>(new CommandDefinition(sql));
            return rooms;
        }

        private async Task<IEnumerable<Booking>> GetBookings(NpgsqlConnection connection)
        {
            const string sql = @"SELECT * FROM ""Bookings""";
            var bookings = await connection.QueryAsync<Booking>(new CommandDefinition(sql));
            return bookings;
        }
    }
}
