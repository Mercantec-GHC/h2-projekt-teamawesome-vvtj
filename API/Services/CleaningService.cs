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

				// Filter rooms that need cleaning. 
				// Here we have taken into account:
				// - Rooms that have never been cleaned.
				// - Rooms that have not been cleaned for a long time.
				// - Rooms that were checked out today or earlier and have not been cleaned since that check out.

				var roomsToClean = rooms
					.Where(r =>
						!r.LastCleaned.HasValue
						|| (DateTime.UtcNow - r.LastCleaned.Value).TotalDays >= 3
						|| bookings.Any(b => b.RoomId == r.Id
											 && b.CheckOut <= DateOnly.FromDateTime(DateTime.UtcNow)
											 && (!r.LastCleaned.HasValue || b.CheckOut > DateOnly.FromDateTime(r.LastCleaned.Value))))
					.ToList();

				var roomToCleanDtos = roomsToClean
                    .GroupBy(r => r.HotelId)
                    .Select(g => new RoomToCleanDto
                    {
                        HotelId = g.Key, 
                        RoomNumbers = g.Select(r => r.RoomNumber).ToList()
                    })
                    .ToList();

                return roomToCleanDtos;
            }

            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                _logger.LogError($"Error updating rooms: {ex.Message}");
                return Enumerable.Empty<RoomToCleanDto>();
            }
        }

        public async Task<List<RoomToCleanDto?>> MarkRoomAsCleanedAsync(List<RoomToCleanDto> roomDtos)
        {
            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            try
            {
                int rowsAffected = 0;

                foreach (var dto in roomDtos)
                {
                    var updated = await _dbContext.Rooms
                        .Where(r => r.HotelId == dto.HotelId && dto.RoomNumbers.Contains(r.RoomNumber))
                        .ExecuteUpdateAsync(s => s.SetProperty(r => r.LastCleaned, _ => DateTime.UtcNow));

                    rowsAffected += updated;
                }

                var totalExpected = roomDtos.Sum(d => d.RoomNumbers.Count());

                if (rowsAffected != totalExpected)
                {
                    _logger.LogInformation($"Incorrect number of rooms updated. Rows affected: {rowsAffected}, expected: {totalExpected}");
                    await transaction.RollbackAsync();
                    return null;
                }

                await transaction.CommitAsync();
                return roomDtos;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError($"Error updating rooms: {ex.Message}");
                return null;
            }
        }
    }
}
