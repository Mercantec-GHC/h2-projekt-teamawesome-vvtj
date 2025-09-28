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
        
    }
}