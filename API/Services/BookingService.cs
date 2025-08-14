using API.Data;
using API.Interfaces;
using DomainModels;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Grpc.Core;


namespace API.Services
{

    public class BookingService : IBookingInterface
    {
        private readonly AppDBContext _dbContext;


        public async Task<IActionResult> CreateBooking(BookingDto bookingDto)
        {
            if (bookingDto == null)
            {
                return new BadRequestObjectResult("User data is required.");
            }

            var nights = (int)(bookingDto.CheckOut.Date - bookingDto.CheckIn.Date).TotalDays;

            var booking = new Booking
            {
                RoomId = bookingDto.RoomId,
                UserId = bookingDto.UserId,
                CheckIn = bookingDto.CheckIn,
                CheckOut = bookingDto.CheckOut,
                NightsCount = nights,
            };

            try
            {
                _dbContext.Bookings.Add(booking);
                await _dbContext.SaveChangesAsync();

                return new OkObjectResult("Booking created successfully.");
            }
            catch (Exception ex)
            {
                return new ObjectResult($"Internal server error: {ex.Message}") { StatusCode = 500 };
            }
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookings()
        {
            // TODO:to make yhis available only for admin-role
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
                    PaymentMethod = b.PaymentMethod,
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
            booking.PriceForNight = dto.PriceForNight;

            await _dbContext.SaveChangesAsync(); 

           
            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                RoomId = booking.RoomId,
                NightsCount = booking.NightsCount,
                GuestsCount = booking.GuestsCount,
                PriceForNight = booking.PriceForNight,
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


        //public async Task<IEnumerable<BookingDto>> GetBookingByHotel(int hotelId)
        //{
        //    var hotelBookings = await _dbContext.Bookings
        //        .Include(b => b.RoomId) // took Room to get HotelId there
        //        .Where(b => b.RoomId.HotelId == hotelId)
        //        .Select(b => new BookingDto
        //        {
        //            Id = b.Id,
        //            UserId = b.UserId,
        //            RoomId = b.RoomId,
        //            NightsCount = b.NightsCount,

        //        })
        //        .ToListAsync();
        //    return hotelBookings;
        //}
    }
}
