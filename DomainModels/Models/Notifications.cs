namespace DomainModels.Models
{
	public class Notifications
	{
		public int Id { get; set; }
		public string? Resource { get; set; }
		public string? Name { get; set; }
		public string? Email { get; set; }
		public string? Message { get; set; }
		public string Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? UpdatedAt { get; set; }
	}
}
