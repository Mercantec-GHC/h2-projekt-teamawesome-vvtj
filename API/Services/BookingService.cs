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

            var room = await roomQuery.FirstOrDefaultAsync();
            var roomType = room.RoomType;

            var UserName = await _dbContext.Users.Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync();
            var guests = Math.Clamp(dto.GuestsCount, 1, roomType.MaxCapacity);
            var nights = Math.Max((dto.CheckOut.Date - dto.CheckIn.Date).Days, 1);
            var pricePerNight = roomType.PricePerNight.GetValueOrDefault(0m);
            var total = pricePerNight * nights;


            var booking = new Booking
            {
                UserId = userId,
                //UserName = dto.UserName,
                RoomId = room.Id,
                CheckIn = dto.CheckIn,
                CheckOut = dto.CheckOut,
                GuestsCount = guests,
                NightsCount = nights,
                TotalPrice = total,
                CreatedAt = DateTime.UtcNow

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
                NightsCount = nights,
                TotalPrice = total

               
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
        //TODO : figure out how to update booking, without deleting other data
        public async Task<bool> UpdateBookingDates(int bookingId, DateTime newCheckIn, DateTime newCheckOut)
        {

            var booking = await _dbContext.Bookings
               .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking is null) return false;
            if (newCheckOut <= newCheckIn) return false;


            booking.CheckIn = newCheckIn;
            booking.CheckOut = newCheckOut;
            booking.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId)
        {

            var today = DateTime.UtcNow.Date;

            var query = _dbContext.Bookings
                .Where(b => b.Room.HotelId == hotelId
                            && b.CheckOut.Date > today)
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
