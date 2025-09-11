using System.Text.Json;
using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace API.Services
{
	public class NotificationService : INotificationService
	{
		private readonly AppDBContext _dbContext;
		private readonly ILogger<NotificationService> _logger;
		private readonly IConfiguration _config;
		public NotificationService(AppDBContext context, ILogger<NotificationService> logger, IConfiguration config)
		{
			_dbContext = context;
			_logger = logger;
			_config = config;
		}

		public async Task SaveSubscriptionAsync(NotificationSubscriptionDto subscription, string userId)
		{
			var oldSubs = _dbContext.NotificationSubscriptions.Where(s => s.UserId == userId);
			_dbContext.NotificationSubscriptions.RemoveRange(oldSubs);

			var entity = new NotificationSubscriptions
			{
				UserId = userId,
				Endpoint = subscription.Url,
				P256DH = subscription.P256dh,
				Auth = subscription.Auth
			};

			_dbContext.NotificationSubscriptions.Add(entity);
			await _dbContext.SaveChangesAsync();
		}

		public async Task SendNotificationAsync(NotificationMessageDto dto)
		{
			try
			{
				var subscriptions = await _dbContext.NotificationSubscriptions.ToListAsync();

				var publicKey = _config["vapid-public"];
				var privateKey = _config["vapid-private"];
				var vapidDetails = new VapidDetails("mailto:<someone@example.com>", publicKey, privateKey);
				var webPushClient = new WebPushClient();

				foreach (var sub in subscriptions)
				{
					var pushSubscription = new PushSubscription(sub.Endpoint, sub.P256DH, sub.Auth);

					var payload = JsonSerializer.Serialize(new
					{
						message = dto.Message
					});

					try
					{
						await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
					}
					catch (Exception ex)
					{
						_logger.LogError(ex, "Error sending push notification to subscription {SubscriptionId}", sub.Id);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error fetching subscriptions from database");
			}
		}
	}
}

