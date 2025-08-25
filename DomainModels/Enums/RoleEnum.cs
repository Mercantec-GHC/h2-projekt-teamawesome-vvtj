namespace DomainModels.Enums;

public enum RoleEnum
{
	Unknown = 1, //a fallback role; asigns in cases, when user exists, but shouldn't have any permissions
	Admin = 2,
	Reception = 3,
	Guest = 4,
	CleaningStaff = 5
}

public static class RoleEnumExtentions
{ public static string GetDescription(this RoleEnum role) => role switch
{
	RoleEnum.Admin => "Administrator with full access",
	RoleEnum.Reception => "Receptionist with limited access; can manage bookings, rooms, cleaning staff",
	RoleEnum.Guest => "Guest with minimal access; can create, view and manage own bookings",
	RoleEnum.CleaningStaff => "Cleaning staff with specific permissions; have only access to room lists that need cleaning and update room's status",
	_ => "Unknown role"
};
}
