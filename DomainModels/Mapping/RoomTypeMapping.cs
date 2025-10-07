using DomainModels.Enums;
using DomainModels.Models;

namespace DomainModels.Mapping
{
    public class RoomTypeMapping
    {
        public RoomTypeDto ToRoomTypeGETdto(RoomType roomType)
        {
            var roomtypeEnum = (RoomTypeEnum)roomType.Id;

            return new RoomTypeDto
            {
                Id = roomType.Id,
                TypeofRoom = roomType.TypeofRoom,
                RoomTypeName = roomType.TypeofRoom.ToString(),
                MaxCapacity = roomType.MaxCapacity,
                Description = roomType.Description,
                HasBalcony = roomType.HasBalcony,
                HasJacuzzi = roomType.HasJacuzzi,
                HasKitchenette = roomType.HasKitchenette,
                HasExtraTowels = roomType.HasExtraTowels,
                HasAirCondition = roomType.HasAirCondition,
                HasGardenView = roomType.HasGardenView,
                HasKettle = roomType.HasKettle,
                HasMiniFridge = roomType.HasMiniFridge,
                HasSeaView = roomType.HasSeaView,
                HasTV = roomType.HasTV,
                HasVault = roomType.HasVault,
                Area = roomType.Area,
                PricePerNight = roomType.PricePerNight,
                ImagePath = roomType.ImagePath,
            };
        }

        public RoomTypePutDto ToRoomTypePUTdto(RoomType roomType)
        {
            return new RoomTypePutDto
            {
                Description = roomType.Description,
                Price = roomType.PricePerNight,
                UpdatedAt = DateTime.UtcNow.AddHours(2)
            };
        }

        public void ApplyRoomTypePutdto(RoomTypePutDto roomTypePutDto, RoomType roomType)
        {
            //Only update if there's a new value
            if (!string.IsNullOrWhiteSpace(roomTypePutDto.Description) && roomTypePutDto.Description != "string")
                roomType.Description = roomTypePutDto.Description;

            //Only update if there's a new value
            if (roomTypePutDto.Price > 0)
                roomType.PricePerNight = roomTypePutDto.Price;

            roomType.UpdatedAt = DateTime.UtcNow.AddHours(2);
        }

    }
}


