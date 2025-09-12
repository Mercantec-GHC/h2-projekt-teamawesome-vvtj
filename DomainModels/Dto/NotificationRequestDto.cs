namespace DomainModels.Dto
{
	public class NotificationMessageDto
	{
		public string Message { get; set; } = string.Empty;
	}

	public class NotificationSubscriptionDto
	{
		public string Url { get; set; }
		public string P256dh { get; set; }
		public string Auth { get; set; }
	}
}
