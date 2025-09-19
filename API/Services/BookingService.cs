using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Enums;
using DomainModels.Mapping;
using DomainModels.Models;
using Humanizer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;


namespace API.Services
{

    public class BookingService : IBookingService
    {
        private readonly AppDBContext _dbContext;
        private readonly SeasonalPricingService? _seasonalPricing;
        public BookingService(AppDBContext context, SeasonalPricingService seasonalPricing)
        {
            _dbContext = context;
            _seasonalPricing = seasonalPricing;

        }

        public async Task<BookingResponseDto> CreateBooking(CreateBookingDto dto)
        {
            var userId = await _dbContext.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            // If user not found, return null
            if (userId == 0)
                return null;

            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelName == dto.HotelName);
            // If hotel not found, aborts and returns null.
            if (hotel == null)
                return null;

            var roomsQuery = await _dbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotel.Id &&
                r.RoomType.TypeofRoom == dto.TypeOfRoom && r.IsAvailable == true)
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

            // If no available room is found, return null
            if (availableRoom == null || availableRoom.RoomType == null)
                return null;

            // Get the room type for price and capacity calculations
            var roomType = availableRoom.RoomType;

            var UserName = await _dbContext.Users.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var guests = Math.Clamp(dto.GuestsCount, 1, roomType.MaxCapacity);

            var nights = Math.Max((dto.CheckOut.DayNumber - dto.CheckIn.DayNumber), 1);

            if (dto.isBreakfast == true)
                roomType.PricePerNight += 200 * guests;

            var pricePerNight = roomType.PricePerNight.GetValueOrDefault(0m);
            decimal finalPrice = await _seasonalPricing.GetSeasonalPrice(pricePerNight, dto.CheckIn.ToDateTime(TimeOnly.MinValue));
            var total = finalPrice * nights;


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
                IsBreakfast = dto.isBreakfast,
            };

            _dbContext.Bookings.Add(booking);
            await _dbContext.SaveChangesAsync();


            return new BookingResponseDto
            {
                UserName = dto.UserName,
                HotelName = hotel.HotelName,
                RoomType = roomType.TypeofRoom,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestsCount = guests,
                IsBreakfast = booking.IsBreakfast,
                NightsCount = nights,
                TotalPrice = total,
            };
        }


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

        public async Task<IEnumerable<GetAvaliableRoomsDto>> GetAvaliableRoomsAsync(string hotelName, DateOnly from, DateOnly to)
        {
            if (to <= from)
                return Enumerable.Empty<GetAvaliableRoomsDto>();

            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelName == hotelName);
            if (hotel == null)
                return Enumerable.Empty<GetAvaliableRoomsDto>();

            var available = await _dbContext.Rooms
              .AsNoTracking()
              .Where(r => r.HotelId == hotel.Id)
              .Where(r => !_dbContext.Bookings.Any(b =>
            b.RoomId == r.Id &&
            b.CheckIn < to && b.CheckOut > from))
                .OrderBy(r => r.RoomNumber)
                .Select(r => new GetAvaliableRoomsDto
                {
                    RoomId = r.Id,
                    RoomNumber = r.RoomNumber,
                    HotelName = r.Hotel.HotelName,
                    RoomTypeName = ((RoomTypeEnum)r.TypeId).ToString()
                })
                .ToListAsync();
            return available;


       }

        public async Task<IEnumerable<BookingByUserDto>> GetBookingByUser(int userId)
        {
            var userBookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new BookingByUserDto
                {
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
                RoomType = booking.Room.RoomType.TypeofRoom,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestsCount = booking.GuestsCount,
                NightsCount = booking.NightsCount,
                TotalPrice = total,
                IsBreakfast = booking.Room.IsBreakfast
            };
        }

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


        public async Task<bool> DeleteBookingById(int id)
        {
            var affected = await _dbContext.Bookings
        .Where(b => b.Id == id)
        .ExecuteDeleteAsync();   //Deletes directly in the database with a single SQL statement, without creating an entity

            return affected > 0;
        }


    }
}
