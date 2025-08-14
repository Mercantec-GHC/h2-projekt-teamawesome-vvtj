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
                .Where(r =>!r.LastCleaned.HasValue || r.LastCleaned.Value <= DateTime.Now.AddDays(-3) ||
                    booking.Any(b =>
                        b.RoomId == r.Id && b.CheckOut <= DateTime.UtcNow))
                .ToList();

            var roomToCleanDtos = roomsToClean.Select(r => new RoomToCleanDto { 
                RoomNumber = r.RoomNumber,
                RoomType = r.RoomType.TypeofRoom,
                HotelName = r.Hotel.HotelName
            });

            return roomToCleanDtos;
        }

        public async Task<bool> MarkRoomAsCleanedAsync(List<int> roomNumbers)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                const string query = @"
                    UPDATE ""Rooms""
                    SET ""LastCleaned"" = @LastCleaned
                    WHERE ""RoomNumber"" = @RoomNumbers";

                var parameters = new
                {
                    LastCleaned = DateTime.UtcNow,
                    RoomNumbers = roomNumbers
                };

                int rowsAffected = await connection.ExecuteAsync(query, parameters, transaction);

                if (rowsAffected != roomNumbers.Count)
                {
                    Console.WriteLine($"Incorrect number of room numbers updated in DB. Rows affected: {rowsAffected}");
                    await transaction.RollbackAsync();

                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                Console.WriteLine($"Error updating rooms: {ex.Message}");
                return false;
            }
        }


        // These temporary methods that i will change to methods that will create Tetyana and Jasmin.
        private async Task<IEnumerable<Room>> GetRooms(NpgsqlConnection connection)
        {
            const string sql = @"
                SELECT r.*, h.*, t.*
                FROM ""Rooms"" r
                JOIN ""Hotels"" h ON r.""HotelId"" = h.""Id""
                JOIN ""RoomTypes"" t ON r.""TypeId"" = t.""Id"" ";

            var rooms = await connection.QueryAsync<Room, Hotel, RoomType, Room>(
                sql,
                (room, hotel, roomType) =>
                {
                    room.Hotel = hotel;
                    room.RoomType = roomType;
                    return room;
                }
            );
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
