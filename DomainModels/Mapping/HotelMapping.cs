using DomainModels.Dto;
using DomainModels.Models;

namespace DomainModels.Mapping
{
    public class HotelMapping
    {
        public HotelDto ToHotelGETdto(Hotel hotel)
        {
            return new HotelDto
            {
                Id = hotel.Id,
                HotelName = hotel.HotelName,
                CityName = hotel.CityName,
                Address = hotel.Address,
                Description = hotel.Description,
                Phone = hotel.Phone,
                Email = hotel.Email,
                WeekdayTime = hotel.WeekdayTime,
                SaturdayTime = hotel.SaturdayTime,
                HolidaysTime = hotel.HolidaysTime
            };
             
        }

        // public Hotel ToHotelPOSTDto(HotelDto createNewHotel)
        // {
        //     return new Hotel
        //     {
        //         HotelName = createNewHotel.HotelName,
        //         CityName = createNewHotel.CityName,
        //         Address = createNewHotel.Address,
        //         Description = createNewHotel.Description,
        //         CreatedAt = DateTime.UtcNow.AddHours(2),
        //         UpdatedAt = DateTime.UtcNow.AddHours(2),
        //         Email = createNewHotel.Email,
        //         Phone = createNewHotel.Phone,
        //         WeekdayTime = createNewHotel.WeekdayTime,
        //         SaturdayTime = createNewHotel.SaturdayTime,
        //         HolidaysTime = createNewHotel.HolidaysTime
        //     };
        // }
        public void TohotelPUTDto(Hotel currentHotel, HotelDto updatedHotel)
        {
            if (!string.IsNullOrWhiteSpace(updatedHotel.HotelName)  && updatedHotel.HotelName != "string")
                currentHotel.HotelName = updatedHotel.HotelName;

            if (!string.IsNullOrWhiteSpace(updatedHotel.CityName) &&updatedHotel.CityName != "string")
                currentHotel.CityName = updatedHotel.CityName;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.Address) && updatedHotel.Address != "string")
                currentHotel.Address = updatedHotel.Address;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.Description)&& updatedHotel.Description != "string")
                currentHotel.Description = updatedHotel.Description;
            
            currentHotel.UpdatedAt = DateTime.UtcNow.AddHours(2);

            if (!string.IsNullOrWhiteSpace(updatedHotel.Email) && updatedHotel.Email != "string")
                currentHotel.Email = updatedHotel.Email;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.Phone) && updatedHotel.Phone != "string")
                currentHotel.Phone = updatedHotel.Phone;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.WeekdayTime) && updatedHotel.WeekdayTime != "string")
                currentHotel.WeekdayTime = updatedHotel.WeekdayTime;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.SaturdayTime) && updatedHotel.SaturdayTime != "string")
                currentHotel.SaturdayTime = updatedHotel.SaturdayTime;
            
            if (!string.IsNullOrWhiteSpace(updatedHotel.HolidaysTime) && updatedHotel.HolidaysTime != "string")
                currentHotel.HolidaysTime = updatedHotel.HolidaysTime;
        }
    }
}