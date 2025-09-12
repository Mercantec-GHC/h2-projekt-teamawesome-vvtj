using DomainModels.Dto;

namespace API.Interfaces
{
	public interface INotificationService
	{
		Task SaveSubscriptionAsync(NotificationSubscriptionDto subscription, string userId);
		Task SendNotificationAsync(NotificationMessageDto dto);
	}
}
