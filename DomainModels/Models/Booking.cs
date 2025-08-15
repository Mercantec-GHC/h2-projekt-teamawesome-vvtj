using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models;
public class Booking
{
    [Key]
    public int Id { get; set; }

    //[ForeignKey("HotelId")]
    //public int HotelId { get; set; }


    [ForeignKey("RoomId")]
    public int RoomId { get; set; } 
 

    public Room RoomBooked { get; set; }

    public required DateTime CheckIn { get; set; }
    public required DateTime CheckOut { get; set; }

    public int NightsCount { get; set; }
    public int GuestsCount { get; set; }
    public double PriceForNight { get; set; }
    public string? PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
  

}
