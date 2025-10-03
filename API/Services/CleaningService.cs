using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
	/// <summary>
	/// Service for handling cleaning operations for hotel rooms.
	/// </summary>
	public class CleaningService : ICleaningService
	{
		private readonly AppDBContext _dbContext;
		private readonly ILogger<CleaningService> _logger;

		/// <summary>
		/// Initializes a new instance of the <see cref="CleaningService"/> class.
		/// </summary>
		/// <param name="configuration">Application configuration.</param>
		/// <param name="context">Database context.</param>
		/// <param name="logger">Logger instance.</param>
		public CleaningService(IConfiguration configuration, AppDBContext context, ILogger<CleaningService> logger)
		{
			_dbContext = context;
			_logger = logger;
		}

		/// <summary>
		/// Retrieves all rooms that require cleaning.
		/// </summary>
		/// <returns>
		/// An enumerable of <see cref="RoomToCleanDto"/> representing rooms to be cleaned, grouped by hotel.
		/// </returns>
		/// <remarks>
		/// Rooms are considered for cleaning if:
		/// - They have never been cleaned.
		/// - They have not been cleaned for at least 3 days.
		/// - They were checked out today or earlier and have not been cleaned since that check out.
		/// </remarks>
		public async Task<IEnumerable<RoomToCleanDto>> GetAllRoomsToCleanAsync()
		{
			using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				// Fetch all bookings and rooms with related RoomType and Hotel.
				List<Booking> bookings = await _dbContext.Bookings.ToListAsync();
				List<Room> rooms = await _dbContext.Rooms.Include(r => r.RoomType).Include(r => r.Hotel).ToListAsync();
 
				// Filter rooms that need cleaning:
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

				// Group rooms to clean by hotel and project to DTO.
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

		/// <summary>
		/// Marks the specified rooms as cleaned by updating their <c>LastCleaned</c> property.
		/// </summary>
		/// <param name="roomDtos">A list of <see cref="RoomToCleanDto"/> containing hotel IDs and room numbers to mark as cleaned.</param>
		/// <returns>
		/// The list of <see cref="RoomToCleanDto"/> if successful; otherwise, <c>null</c>.
		/// </returns>
		/// <remarks>
		/// All rooms in the provided DTOs must be updated successfully, otherwise the transaction is rolled back.
		/// </remarks>
		public async Task<List<RoomToCleanDto?>> MarkRoomAsCleanedAsync(List<RoomToCleanDto> roomDtos)
		{
			using var transaction = await _dbContext.Database.BeginTransactionAsync();

			try
			{
				int rowsAffected = 0;

				// Update LastCleaned for each room in the provided DTOs.
				foreach (var dto in roomDtos)
				{
					var updated = await _dbContext.Rooms
						.Where(r => r.HotelId == dto.HotelId && dto.RoomNumbers.Contains(r.RoomNumber))
						.ExecuteUpdateAsync(s => s.SetProperty(r => r.LastCleaned, _ => DateTime.UtcNow));

					rowsAffected += updated;
				}

				var totalExpected = roomDtos.Sum(d => d.RoomNumbers.Count());

				// Ensure all intended rooms were updated.
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
