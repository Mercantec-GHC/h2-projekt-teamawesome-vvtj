using DomainModels.Enums;
using DomainModels.Models;
//POST
public class RoomCreateDto
{
     public int Id { get; set; }
    //public required int GuestCount { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsBreakfast { get; set; }
    public DateTime AvailableFrom { get; set; }
    public int RoomNumber { get; set; }
    public int RoomtypeId { get; set; }
    public int HotelId { get; set; }
}