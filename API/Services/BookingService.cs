using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Mapping;
using DomainModels.Models;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Services
{

    public class BookingService : IBookingService
    {
        private readonly AppDBContext _dbContext;
        public BookingService(AppDBContext context)
        {
            _dbContext = context;
        }

        public async Task<BookingResponseDto> CreateBooking(CreateBookingDto dto)
        {
            var userId = await _dbContext.Users
                .Where(u => u.UserName == dto.UserName)
                .Select(u => u.Id)
                .FirstOrDefaultAsync();


            var hotel = await _dbContext.Hotels
                .AsNoTracking()
                .FirstOrDefaultAsync(h => h.HotelName == dto.HotelName);


            var roomQuery = _dbContext.Rooms
                .Include(r => r.RoomType)
                .Where(r => r.HotelId == hotel.Id);

            if (!string.IsNullOrWhiteSpace(dto.TypeOfRoom))
                roomQuery = roomQuery.Where(r => r.RoomType.TypeofRoom == dto.TypeOfRoom);

            var rooms = await roomQuery.ToListAsync();

            var room = rooms.FirstOrDefault(r => r.IsAvailable);

            var roomType = room.RoomType;
                        
            var UserName = await _dbContext.Users.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();

            var guests = Math.Clamp(dto.GuestsCount, 1, roomType.MaxCapacity);
           
            var nights = Math.Max((dto.CheckOut.DayNumber - dto.CheckIn.DayNumber), 1);

            if (dto.isBreakfast == true)
                roomType.PricePerNight += 200 * guests;
                        
            var pricePerNight = roomType.PricePerNight.GetValueOrDefault(0m);
            var total = pricePerNight * nights;


            var booking = new Booking
            {
                UserId = userId,

                RoomId = room.Id,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                GuestsCount = guests,
                NightsCount = nights,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow,
                IsPaid = true
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
                IsBreakfast = room.IsBreakfast,
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

                    UserName = b.User.UserName,
                    RoomId = b.RoomId,
                    HotelName = b.Room.Hotel.HotelName,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut
                })
                .ToListAsync();

            return bookings;
        }


        public async Task<IEnumerable<BookingDto>> GetBookingByUser(int userId)
        {
            var userBookings = await _dbContext.Bookings
                .Where(b => b.UserId == userId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserName = b.User.UserName,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    NightsCount = b.NightsCount,
                    GuestsCount = b.GuestsCount,

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

           

            if (booking == null || !booking.Room.IsAvailable)
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
           

            var total = pricePerNight * nights;
            booking.TotalPrice = total;

            await _dbContext.SaveChangesAsync();

            return new BookingResponseDto
            {
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
