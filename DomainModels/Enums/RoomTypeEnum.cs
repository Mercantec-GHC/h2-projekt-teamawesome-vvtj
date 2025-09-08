namespace DomainModels.Enums;

public enum RoomTypeEnum
{
	Standard = 1,
	Family = 2,
	Single = 3,
	Royal = 4,
	Penthouse = 5
}

// public static class RoomTypeExtenstion
// {
//     public static string GetDescription(this RoomTypeEnum roomType) => roomType switch
//     {
//         RoomTypeEnum.Standard => "A comfortable room. Ideal for solo travelers or couples seeking simplicity and value.",
//         RoomTypeEnum.Family => "Spacious and cozy, designed to accommodate families with multiple beds.",
//         RoomTypeEnum.Single => "Compact and efficient, perfect for one guest looking for a quiet and affordable stay.",
//         RoomTypeEnum.Royal => "Elegant and refined, offering upscale features for a luxurious experience.",
//         RoomTypeEnum.Penthouse => "The top-tier suite with panoramic views. Maximum comfort�reserved for the ultimate stay."
//     };
// }
