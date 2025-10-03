using DomainModels.Enums;
using DomainModels.Models;

public class RoomsDto
{
    public int Id { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBreakfast { get; set; }
    public DateTime AvailableFrom { get; set; }
    public int RoomNumber { get; set; }
    public RoomType RoomType { get; set; }
    public string RoomTypeName { get; set; }
    public string HotelName { get; set; }
    public int HotelId { get; set; }
}

//GET
// public class RoomsViewDto
// {
//     public int Id { get; set; }
//     public required int GuestCount { get; set; }
//     public bool IsAvailable { get; set; }
//     public bool IsBreakfast { get; set; }
//     public DateTime AvailableFrom { get; set; }

//     public int TypeId { get; set; }
//     public RoomType RoomType { get; set; }
// }

// //POST
// public class RoomsCreateDto
// {
//     public int RoomNumber { get; set; }
//     public required int GuestCount { get; set; }
//     public bool IsAvailable { get; set; }
//     public bool IsBreakfast { get; set; }
//     public DateTime AvailableFrom { get; set; }

//     public int TypeId { get; set; }
//     public RoomType RoomType { get; set; }
// }

