using DomainModels.Dto;

namespace Blazor.Services
{
	public partial class APIService
	{
		public async Task<bool> SubscribeToPushNotificationsAsync(NotificationSubscriptionDto subscription)
		{
			try
			{
				var response = await PostAsJsonAsync("api/Notifications/subscribe", subscription);
				response.EnsureSuccessStatusCode();
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while subscribing to push notifications: {Message}", ex.Message);
				return false;
			}
		}

		public async Task<bool> SendNotificationAsync(string message)
		{
			try
			{
				var request = new NotificationMessageDto { Message = message };
				var response = await PostAsJsonAsync("api/Notifications/send", request);
				response.EnsureSuccessStatusCode();
				return response.IsSuccessStatusCode;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Exception while sending push notification: {Message}", ex.Message);
				return false;
			}
		}
	}
}
