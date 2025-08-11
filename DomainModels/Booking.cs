using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainModels;
public class Booking
{
	[Key]
	public required int Id { get; set; }
	
	public int RoomId { get; set; }
	[ForeignKey("RoomId")]
	public Room RoomBooked { get; set; }

	public required DateTime CheckIn { get; set; }
	public required DateTime CheckOut { get; set; }
	public int NightsCount { get; set; } 
	public int GuestsCount { get; set; } 
	public string? Payment {  get; set; }
	public bool IsPaid { get; set; }
	public required int UserId { get; set; }

	[ForeignKey("UserId")]
	public User User { get; set; }

}
