namespace DomainModels.Models
{
	public class NotificationSubscriptions
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string Endpoint { get; set; }
		public string P256DH { get; set; }
		public string Auth { get; set; }
	}
}
