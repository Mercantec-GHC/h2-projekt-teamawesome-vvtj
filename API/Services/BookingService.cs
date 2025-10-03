using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Enums;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;


namespace API.Services
{

    public class BookingService : IBookingService
    {
        private readonly AppDBContext _dbContext;
        private readonly SeasonalPricingService? _seasonalPricing;
        private readonly IEmailService _emailService;
        private readonly ILogger<BookingService> _logger;


        /// <summary>
        /// Initializes a new instance of the <see cref="BookingService"/> class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="seasonalPricing">The seasonal pricing service.</param>
        /// <param name="emailService">The email service.</param>
        /// <param name="logger">The logger instance.</param>
        public BookingService(AppDBContext context, SeasonalPricingService seasonalPricing, IEmailService emailService, ILogger<BookingService> logger)
        {
            _dbContext = context;
            _seasonalPricing = seasonalPricing;
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new booking or returns a preview of the booking details without saving, based on the provided parameters.
        /// </summary>
        /// <param name="dto">The booking details to create.</param>
        /// <param name="preview">If true, returns a preview of the booking without saving it.</param>
        /// <returns>A <see cref="BookingResponseDto"/> containing the booking details, or null if booking could not be created.</returns>
        public async Task<BookingResponseDto> CreateBooking(CreateBookingDto dto, bool preview = false)
        {
            var userId = await _dbContext.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (userId == 0)
                return null;

            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelName == dto.HotelName);

            if (hotel == null)
                return null;

            var roomsQuery = await _dbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotel.Id &&
                 r.TypeId == dto.RoomTypeId && r.IsAvailable == true)
                .Select(r => new { r.Id, r.RoomType })
                 .ToListAsync();

            //if there are no rooms with chosen type in the hotel
            if (roomsQuery.Count == 0)
                return null;

            var roomIds = roomsQuery.Select(r => r.Id).ToList();


            // Find rooms that are not booked for the requested dates (single query for overlaps)
            var overlappingRoomIds = await _dbContext.Bookings
                .Where(b =>
                    roomIds.Contains(b.RoomId) &&
                    (
                        (dto.CheckIn >= b.CheckIn && dto.CheckIn < b.CheckOut) ||
                        (dto.CheckOut > b.CheckIn && dto.CheckOut <= b.CheckOut) ||
                        (dto.CheckIn <= b.CheckIn && dto.CheckOut >= b.CheckOut)
                    )
                )
                .Select(b => b.RoomId)
                .Distinct()
                .ToListAsync();

            var availableRoomId = roomIds.Except(overlappingRoomIds).FirstOrDefault();

            if (availableRoomId == 0)
                return null; // No available rooms found

            var availableRoom = roomsQuery.FirstOrDefault(r => r.Id == availableRoomId);

            if (availableRoom == null || availableRoom.RoomType == null)
                return null;

            // Get the room type for price and capacity calculations
            var roomType = availableRoom.RoomType;

            var UserName = await _dbContext.Users.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var guests = Math.Clamp(dto.GuestsCount, 1, roomType.MaxCapacity);
            var nights = Math.Max((dto.CheckOut.DayNumber - dto.CheckIn.DayNumber), 1);

            if (dto.IsBreakfast == true)
                roomType.PricePerNight += 200 * guests;

            var pricePerNight = roomType.PricePerNight.GetValueOrDefault(0m);

            decimal finalPrice = await _seasonalPricing.GetSeasonalPrice(pricePerNight, dto.CheckIn.ToDateTime(TimeOnly.MinValue));

            var total = finalPrice * nights;

            var responseDto = new BookingResponseDto
            {
                UserName = dto.UserName,
                HotelName = hotel.HotelName,
                TypeOfRoom = roomType.TypeofRoom,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                GuestsCount = guests,
                IsBreakfast = dto.IsBreakfast,
                NightsCount = nights,
                TotalPrice = total,
            };


            if (!preview)
            {
                var booking = new Booking
                {
                    UserId = userId,
                    RoomId = availableRoom.Id,
                    CheckIn = dto.CheckIn,
                    CheckOut = dto.CheckOut,
                    GuestsCount = guests,
                    NightsCount = nights,
                    TotalPrice = total,
                    CreatedAt = DateTime.UtcNow,
                    IsBreakfast = dto.IsBreakfast,
                };

                _dbContext.Bookings.Add(booking);

                await _dbContext.SaveChangesAsync();

                // Send booking confirmation email
                await SendBookingConfirmationNotification(userId, responseDto);
            }

            return responseDto;

        }

        private async Task SendBookingConfirmationNotification(int userId, BookingResponseDto responseDto)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning($"Booking confirmation email skipped: User with ID {userId} not found.");
                return;
            }

            try
            {
                await _emailService.SendBookingConfirmationEmailAsync(new EmailFormDto
                {
                    Name = user.UserName,
                    Email = user.Email
                }, responseDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send booking confirmation email to {user.Email} (ID: {userId}).");
            }
        }

        /// <summary>
        /// Retrieves all bookings in the system.
        /// </summary>
        /// <returns>A collection of <see cref="GetBookingsDto"/> representing all bookings.</returns>
        public async Task<IEnumerable<GetBookingsDto>> GetAllBookings()
        {
            var bookings = await _dbContext.Bookings
                .Select(b => new GetBookingsDto
                {
                    Id = b.Id,
                    CreatedAt = b.CreatedAt,
                    UpdatedAt = b.UpdatedAt ?? DateTime.MinValue,
                    UserName = b.User.UserName,
                    RoomId = b.RoomId,
                    HotelName = b.Room.Hotel.HotelName,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut
                })
                .ToListAsync();

            return bookings;
        }


        /// <summary>
        /// Retrieves a list of available rooms for a specified hotel and date range.
        /// </summary>
        /// <param name="hotelName">The name of the hotel to search for available rooms.</param>
        /// <param name="from">The start date of the availability period.</param>
        /// <param name="to">The end date of the availability period.</param>
        /// <returns>A collection of <see cref="GetAvailableRoomsDto"/> representing available rooms for the given hotel and date range.</returns>
        public async Task<IEnumerable<GetAvailableRoomsDto>> GetAvailableRoomsAsync(string hotelName, DateOnly from, DateOnly to)
        {
            if (to <= from)
                return Enumerable.Empty<GetAvailableRoomsDto>();

            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                 .FirstOrDefaultAsync(h => h.HotelName.ToLower().Contains(hotelName.ToLower()));
            if (hotel == null)
                return Enumerable.Empty<GetAvailableRoomsDto>();


            var available = await _dbContext.Rooms
              .AsNoTracking()
              .Where(r => r.HotelId == hotel.Id)
              .Where(r => !_dbContext.Bookings.Any(b =>
            b.RoomId == r.Id &&
            b.CheckIn < to && b.CheckOut > from))
                .OrderBy(r => r.RoomNumber)
                .Select(r => new GetAvailableRoomsDto
                {
                    RoomId = r.Id,
                    RoomNumber = r.RoomNumber,
                    HotelName = r.Hotel.HotelName,
                    RoomTypeName = ((RoomTypeEnum)r.TypeId).ToString()                 
                })
                .ToListAsync();
            return available;
        }


        /// <summary>
        /// Retrieves all bookings for a specific user by their ID.
        /// </summary>
        /// <param name="userId">Id of the user.</param>
        /// <returns>A collection of <see cref="BookingByUserDto"/> representing the user's bookings.</returns>        
        public async Task<IEnumerable<BookingByUserDto>> GetBookingByUser(int userId)
        {
            var userBookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                 .OrderByDescending(b => b.Id)
                .Select(b => new BookingByUserDto
                {
                    Id = b.Id,
                    CreatedAt = b.CreatedAt,
                    UserName = b.User.UserName,
                    HotelName = b.Room.Hotel.HotelName,
                    RoomType = b.Room.RoomType.TypeofRoom,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    NightsCount = b.NightsCount,
                    GuestsCount = b.GuestsCount,
                    TotalPrice = b.TotalPrice,
                    IsBreakfast = b.IsBreakfast
                })
                .ToListAsync();

            return userBookings;
        }


        /// <summary>
        /// Updates the check-in and check-out dates for an existing booking.
        /// </summary>
        /// <param name="bookingId">Id of the booking to update.</param>
        /// <param name="newCheckIn">The new check-in date.</param>
        /// <param name="newCheckOut">The new check-out date.</param>
        /// <returns>
        /// A <see cref="BookingResponseDto"/> containing the updated booking details, or null if the booking was not found.
        /// </returns>
        public async Task<BookingResponseDto?> UpdateBookingDatesAsync(int bookingId, DateOnly newCheckIn, DateOnly newCheckOut)
        {
            var booking = await _dbContext.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.User)
                .Include(b => b.Room.Hotel)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
                return null;

            booking.CheckIn = newCheckIn;
            booking.CheckOut = newCheckOut;

            var nights = Math.Max((newCheckOut.DayNumber - newCheckIn.DayNumber), 1);
            booking.NightsCount = nights;


            var pricePerNight = booking.Room.RoomType.PricePerNight.GetValueOrDefault(0m);
            if (booking.Room.IsBreakfast)
            {
                pricePerNight += 200 * booking.GuestsCount;
            }

            decimal finalPrice = await _seasonalPricing.GetSeasonalPrice(pricePerNight, booking.CheckIn.ToDateTime(TimeOnly.MinValue));
            var total = finalPrice * nights;
            booking.TotalPrice = total;
            booking.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();

            return new BookingResponseDto
            {
                UpdatedAt = booking.UpdatedAt,
                UserName = booking.User.UserName,
                HotelName = booking.Room.Hotel.HotelName,
                TypeOfRoom = booking.Room.RoomType.TypeofRoom,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestsCount = booking.GuestsCount,
                NightsCount = booking.NightsCount,
                TotalPrice = total,
                IsBreakfast = booking.Room.IsBreakfast
            };
        }

        /// <summary>
        /// Retrieves all bookings for a specified hotel by its id.
        /// </summary>
        /// <param name="hotelId">Id of the hotel.</param>
        /// <returns>A collection of <see cref="BookingDto"/> representing the bookings for the hotel.</returns>
        public async Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId)
        {

            var today = DateTime.UtcNow.Date;

            var query = _dbContext.Bookings
                .Where(b => b.Room.HotelId == hotelId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.UserName,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    GuestsCount = b.GuestsCount,
                    HotelId = b.Room.HotelId,
                    HotelName = b.Room.Hotel.HotelName,
                    RoomType = b.Room.RoomType.TypeofRoom,
                    isBreakfast = b.Room.IsBreakfast,
                })
            .AsNoTracking();

            return await query.ToListAsync();

        }


        /// <summary>
        /// Deletes a booking by its Id.
        /// </summary>
        /// <param name="id">Id of the booking to delete.</param>
        /// <returns>True if the booking was deleted; otherwise, false.</returns>
        public async Task<bool> DeleteBookingById(int id)
        {
            var affected = await _dbContext.Bookings
                .Where(b => b.Id == id)
                .ExecuteDeleteAsync();   //Deletes directly in the database with a single SQL statement, without creating an entity

            return affected > 0;
        }


        /// <summary>
        /// Deletes a booking for the specified user if the booking's check-in date is in the future.
        /// </summary>
        /// <param name="bookingId">The ID of the booking to delete.</param>
        /// <param name="userId">The ID of the user who owns the booking.</param>
        /// <returns>True if the booking was deleted; otherwise, false.</returns>
        public async Task<bool> DeleteMyBooking(int bookingId, int userId)
        {
            var affected = await _dbContext.Bookings
                .Where(b => b.Id == bookingId
                            && b.UserId == userId
                            && b.CheckIn > DateOnly.FromDateTime(DateTime.UtcNow))
                .ExecuteDeleteAsync();

            return affected > 0;
        }
    }
}
