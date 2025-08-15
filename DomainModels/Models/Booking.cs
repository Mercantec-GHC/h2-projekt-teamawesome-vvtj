using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models;
public class Booking
{
    [Key]
    public int Id { get; set; }

    

    [ForeignKey("RoomId")]
    public int RoomId { get; set; }


     public required DateTime CheckIn { get; set; }
    public required DateTime CheckOut { get; set; }

    public int NightsCount { get; set; }
    public int GuestsCount { get; set; }
   //public double TotalPrice {get; set;} //TODO
    public string? Payment { get; set; } = default!;
    public bool IsPaid { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }


}
