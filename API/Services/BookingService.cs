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


            var nights = (bookingDto.CheckOut.Date - bookingDto.CheckIn.Date).Days;
            if (nights <= 0)
                throw new InvalidOperationException("Check-out must be after check-in.");


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

            var rt = room.RoomType ?? throw new InvalidOperationException("Room type not found.");
            if (bookingDto.CheckIn.Date < DateTime.UtcNow.Date)
                throw new InvalidOperationException("Check-in date cannot be in the past.");

            var guests = bookingDto.GuestsCount > 0 ? bookingDto.GuestsCount : 1;
            
            decimal pricePerNight = (decimal)room.RoomType!.PricePerNight;
            decimal total = pricePerNight * bookingDto.NightsCount;
            
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
               
                TotalPrice = (decimal)newBooking.TotalPrice,
            };

        }

        public async Task<IEnumerable<GetBookingsDto>> GetAllBookings()
        {
            // TODO:to make yhis available only for admin-role
            
            var bookings = await _dbContext.Bookings
                .Select(b => new GetBookingsDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    UserName = b.User.UserName,
                    RoomId = b.RoomId,
                    RoomType = b.Room.RoomType.TypeofRoom,
                    HotelId = b.Room.HotelId,
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
            booking.NightsCount = (int)(booking.CheckOut.Date - booking.CheckIn.Date).TotalDays;




            var room = await _dbContext.Rooms
      .Include(r => r.RoomType)
      .Include(r => r.Hotel)
      .FirstOrDefaultAsync(r => r.Id == booking.RoomId);

            // TODO: check role (user, who booked, or admin, or receptionist)
            booking.CheckIn = dto.CheckIn;
            booking.CheckOut = dto.CheckOut;
            booking.GuestsCount = dto.GuestsCount;
            decimal pricePerNight = (decimal)room.RoomType!.PricePerNight;
            decimal total = pricePerNight * booking.NightsCount;

            await _dbContext.SaveChangesAsync();


            return new BookingDto
            {
                Id = booking.Id,
                UserId = booking.UserId,
                RoomId = booking.RoomId, 
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut, 
                NightsCount = booking.NightsCount,
                GuestsCount = booking.GuestsCount,
               
            };
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
            IsPaid = b.IsPaid,
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
