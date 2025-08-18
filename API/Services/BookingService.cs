using API.Data;
using API.Interfaces;
using DomainModels.Models;
using DomainModels.Dto;
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



        public async Task<BookingDto> CreateBooking(BookingDto bookingDto)
        {
            if (bookingDto == null)
                throw new ArgumentNullException(nameof(bookingDto), "User data is required.");


            //  bookingDto.Id = 0;

            bookingDto.NightsCount = (int)(bookingDto.CheckOut.Date - bookingDto.CheckIn.Date).TotalDays;




            var room = await _dbContext.Rooms
      .Include(r => r.RoomType)
      .Include(r => r.Hotel)
      .FirstOrDefaultAsync(r => r.Id == bookingDto.RoomId);

            if (room == null)
                throw new InvalidOperationException("Room not found.");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == bookingDto.UserId);

            if (user == null)
                throw new InvalidOperationException("User not found.");

            var pricePerNight = room.RoomType?.PricePerNight ?? 0m;     // decimal
            var total = (double)pricePerNight * bookingDto.NightsCount;
            var newBooking = new Booking
            {
                RoomId = bookingDto.RoomId,
                UserId = bookingDto.UserId,
                CheckIn = bookingDto.CheckIn,
                CheckOut = bookingDto.CheckOut,
                NightsCount = bookingDto.NightsCount,
                GuestsCount = bookingDto.GuestsCount,
                TotalPrice = total,
                IsPaid = bookingDto.IsPaid,
                Payment = bookingDto.Payment
            };


            _dbContext.Bookings.Add(newBooking);
            await _dbContext.SaveChangesAsync();

            return new BookingDto

            {
                Id = newBooking.Id,
                UserId = user.Id,
                UserName = user.UserName,
                RoomId = room.Id,
                RoomType = room.RoomType?.TypeofRoom ?? string.Empty,
                HotelId = room.HotelId,
                HotelName = room.Hotel.HotelName,
                CheckIn = newBooking.CheckIn,
                CheckOut = newBooking.CheckOut,
                NightsCount = newBooking.NightsCount,
                GuestsCount = newBooking.GuestsCount,
                TotalPrice = newBooking.TotalPrice
            };




        }

        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            // TODO:to make yhis available only for admin-role
            var bookingsdb = await _dbContext.Bookings.ToListAsync();

            var bookings = await _dbContext.Bookings
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,

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
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    NightsCount = b.NightsCount,
                    GuestsCount = b.GuestsCount,
                    Payment = b.Payment,
                    IsPaid = b.IsPaid

                })
                .ToListAsync();
            return userBookings;
        }

        public async Task<BookingDto?> UpdateBooking(int bookingId, BookingDto dto)
        {
            var booking = await _dbContext.Bookings.FindAsync(bookingId);


            if (booking == null)
                return null;

            // TODO: check role (user, who booked, or admin, or receptionist)
            booking.NightsCount = dto.NightsCount;
            booking.GuestsCount = dto.GuestsCount;
            // booking.PriceForNight = dto.PriceForNight; //does not exist in the db

            await _dbContext.SaveChangesAsync();


            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                NightsCount = booking.NightsCount,
                GuestsCount = booking.GuestsCount,
                //     PriceForNight = booking.PriceForNight,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,

            };
        }
        public async Task<bool> DeleteBookingById(int id)
        {
            var booking = await _dbContext.Bookings.FindAsync(id);

            // TODO: check admin-role, or maybe user, who made booking
            _dbContext.Bookings.Remove(booking);
            await _dbContext.SaveChangesAsync();// not sure, if it nessesary
            return true;
        }


        public async Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId)
        {
            var hotelBookings = await _dbContext.Bookings
                .Include(b => b.RoomId) // took Room to get HotelId there
                .Where(b => b.Room.HotelId == hotelId)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,

                })
                .ToListAsync();
            return hotelBookings;
        }
    }
}
