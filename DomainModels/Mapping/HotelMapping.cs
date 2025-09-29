using DomainModels.Dto;
using DomainModels.Enums;
using DomainModels.Models;

namespace DomainModels.Mapping
{
    public class HotelMapping
    {
        public void TohotelPUTDto(Hotel currentHotel, HotelDto updatedHotel)
        {
            currentHotel.HotelName = updatedHotel.HotelName;
            currentHotel.CityName = updatedHotel.CityName;
            currentHotel.Address = updatedHotel.Address;
            currentHotel.Description = updatedHotel.Description;
            currentHotel.UpdatedAt = DateTime.UtcNow.AddHours(2);
            currentHotel.Email = updatedHotel.Email;
            currentHotel.Phone = updatedHotel.Phone;
            currentHotel.WeekdayTime = updatedHotel.WeekdayTime;
            currentHotel.SaturdayTime = updatedHotel.SaturdayTime;
            currentHotel.HolidaysTime = updatedHotel.HolidaysTime;
        }

        public void ToHotelPatchDto(Hotel currentHotel, HotelPutDto hotelPutDto)
        {
            currentHotel.HotelName = hotelPutDto.HotelName;
            currentHotel.CityName = hotelPutDto.CityName;
            currentHotel.Address = hotelPutDto.Address;
            currentHotel.Description = hotelPutDto.Description;
        }
    }
}