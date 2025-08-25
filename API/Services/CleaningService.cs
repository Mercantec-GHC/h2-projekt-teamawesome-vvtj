using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class CleaningService : ICleaningService
    {
        private readonly AppDBContext _dbContext;
        private readonly ILogger<CleaningService> _logger;

        public CleaningService(IConfiguration configuration, AppDBContext context, ILogger<CleaningService> logger)
        {
            _dbContext = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RoomToCleanDto>> GetAllRoomsToCleanAsync()
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                List<Booking> bookings = await _dbContext.Bookings.ToListAsync();
                List<Room> rooms = await _dbContext.Rooms.Include(r => r.RoomType).Include(r => r.Hotel).ToListAsync();

                // Filter rooms that need cleaning
                var roomsToClean = rooms
                    .Where(r => !r.LastCleaned.HasValue || (DateTime.UtcNow - r.LastCleaned.Value).TotalDays >= 3 ||
                        bookings.Any(b => b.RoomId == r.Id && b.CheckOut <= DateTime.UtcNow))
                    .ToList();

                var roomToCleanDtos = roomsToClean.Select(r => new RoomToCleanDto
                {
                    RoomNumber = r.RoomNumber,
                    RoomType = r.RoomType.TypeofRoom,
                    HotelName = r.Hotel.HotelName
                });

                return roomToCleanDtos;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Error updating rooms: {ex.Message}");
                return Enumerable.Empty<RoomToCleanDto>();
            }
        }

        public async Task<bool> MarkRoomAsCleanedAsync(List<int> roomNumbers)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                int rowsAffected = await _dbContext.Rooms
                    .Where(r => roomNumbers.Contains(r.RoomNumber))
                    .ExecuteUpdateAsync(s => s.SetProperty(r => r.LastCleaned, _ => DateTime.UtcNow));

                if (rowsAffected != roomNumbers.Count)
                {

                    _logger.LogInformation($"Incorrect number of room numbers updated in DB. Rows affected: {rowsAffected}");
                    await transaction.RollbackAsync();
                    return false;
                }

                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error updating rooms: {ex.Message}");
                return false;
            }
        }

    }
}
