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
                MaxCapacity = roomType.MaxCapacity,
                Description = roomtypeEnum.GetDescription(),
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
            };
        }
    }
}