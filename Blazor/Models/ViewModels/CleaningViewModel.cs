namespace Blazor.Models.ViewModels
{
	public class CleaningViewModel
	{
		public string? Hotel { get; set; }
		public List<int> RoomNumbers { get; set; } = new();
	}
}
