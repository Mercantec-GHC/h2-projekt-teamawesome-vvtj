using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels.Models;
public class Booking : Common
{

    [ForeignKey("RoomId")]
    public int RoomId { get; set; }
    public Room? Room { get; set; } = default!;

    [Required]

    public required DateOnly CheckIn { get; set; } 
    public required DateOnly CheckOut { get; set; }
    public int NightsCount { get; set; }
    public int GuestsCount { get; set; }

    public decimal? TotalPrice { get; set; }
  
    public bool IsBreakfast { get; set; }
    public int UserId { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}

