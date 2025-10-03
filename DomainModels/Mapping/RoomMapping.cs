using DomainModels.Models;

namespace DomainModels.Mapping
{
    public class RoomMapping
    {
        public RoomsDto ToRoomGETdto(Room room)
        {
            return new RoomsDto
            {
                Id = room.Id,
                IsAvailable = room.IsAvailable,
                IsBreakfast = room.IsBreakfast,
                AvailableFrom = room.AvailableFrom,
                RoomNumber = room.RoomNumber,
                RoomType = room.RoomType,
                RoomTypeName = room.RoomType?.TypeofRoom.ToString(),
                HotelName = room.Hotel?.HotelName,
                HotelId = room.Hotel?.Id ?? 0,
            };
        }
    }
}