using DomainModels.Dto;

namespace API.Interfaces
{
	public interface INotificationService
	{
		Task SaveSubscriptionAsync(NotificationSubscriptionDto subscription, string userId);
		Task SendPushNotificationAsync(NotificationMessageDto dto);
		Task<bool> SaveEmailNotificationAsync(EmailFormDto dto);
		Task<bool> UpdateNotificationStatusAsync(NotificationStatusDto dto);
		Task<GetNotificationsDto> GetAllNotificationsAsync();
	}
}
