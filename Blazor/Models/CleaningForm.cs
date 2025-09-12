using System.ComponentModel.DataAnnotations;

namespace Blazor.Models
{
	public class CleaningForm
	{
		[Required(ErrorMessage = "Please select a hotel")]
		public int? SelectedHotelId { get; set; }
		[Required(ErrorMessage = "Please enter room numbers")]
		public string RoomNumbersInput { get; set; } = string.Empty;
	}
}
