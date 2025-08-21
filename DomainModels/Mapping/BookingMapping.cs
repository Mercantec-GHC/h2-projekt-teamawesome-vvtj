using DomainModels.Dto;
using DomainModels.Models;



namespace DomainModels.Mapping
{
    public class BookingMapping
    {
      
            public static Booking MapToEntity(BookingDto dto, decimal total)
            {
                return new Booking
                {
                    RoomId = dto.RoomId,
                    UserId = dto.UserId,
                    UserName = dto.UserName,
                    CheckIn = dto.CheckIn,
                    CheckOut = dto.CheckOut,
                    GuestsCount = dto.GuestsCount,
                    NightsCount = dto.NightsCount,
                    TotalPrice = total,
                    CreatedAt = DateTime.UtcNow
                };
            }
        public static BookingResponseDto ToResponseDto(Booking booking, string hotelName, string typeOfRoom, decimal total, int guests)
        {
            return new BookingResponseDto
            {
                UserName = booking.UserName,
                HotelName = hotelName,
                RoomType = typeOfRoom,
                CheckIn = booking.CheckIn,
                CheckOut = booking.CheckOut,
                GuestsCount = guests,
                NightsCount = booking.NightsCount,
                TotalPrice = total
            };
        }
             public static BookingDto MapToDto(Booking entity)
            {
                return new BookingDto
                {
                    
                    UserName = entity.UserName,
                    CheckIn = entity.CheckIn,
                    CheckOut = entity.CheckOut,
                    TotalPrice = entity.TotalPrice,
                    GuestsCount = entity.GuestsCount,
                    NightsCount = entity.NightsCount,
                   
                };
            }
        
    }
}
