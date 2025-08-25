// using DomainModels.Models;

// namespace DomainModels.Mapping
// {
//     public class RoomTypeMapping
//     {
//         public RoomTypeDto ToRoomTypeGETdto(RoomType roomType)
//         {
//             var newRoomTypesListDto = roomtypes
//             .Select(rt =>
//             {
//                 if (!RoomTypeEnumHelper.TryToConvert(rt.TypeofRoom, out var roomTypeEnum))
//                     return null;
//                 return new RoomTypeDto
//                 {
//                     Id = rt.Id,
//                     TypeofRoom = roomTypeEnum.ToString(),
//                     MaxCapacity = rt.MaxCapacity,
//                     Description = rt.Description,
//                     HasBalcony = rt.HasBalcony,
//                     HasJacuzzi = rt.HasJacuzzi,
//                     HasKitchenette = rt.HasKitchenette,
//                     HasExtraTowels = rt.HasExtraTowels,
//                     HasAirCondition = rt.HasAirCondition,
//                     HasGardenView = rt.HasGardenView,
//                     HasKettle = rt.HasKettle,
//                     HasMiniFridge = rt.HasMiniFridge,
//                     HasSeaView = rt.HasSeaView,
//                     HasTV = rt.HasTV,
//                     HasVault = rt.HasVault,
//                 };
//             }).ToList();
//             return newRoomTypesListDto;
//         }
//         }
//     }
// }