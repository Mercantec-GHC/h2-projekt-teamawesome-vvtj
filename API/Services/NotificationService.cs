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
		private readonly IEmailService _emailService;
		public NotificationService(AppDBContext context, ILogger<NotificationService> logger, IConfiguration config, IEmailService emailService)
		{
			_dbContext = context;
			_logger = logger;
			_config = config;
			_emailService = emailService;
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

		public async Task SendPushNotificationAsync(NotificationMessageDto dto)
		{
			try
			{
				var subscriptions = await _dbContext.NotificationSubscriptions.ToListAsync();

				var publicKey = _config["vapid-public"];
				var privateKey = _config["vapid-private"];

				if (string.IsNullOrEmpty(publicKey) || string.IsNullOrEmpty(privateKey))
				{
					_logger.LogWarning("VAPID keys are missing. Push notifications will not be sent.");
					return;
				}

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

		public async Task<bool> SaveEmailNotificationAsync(EmailFormDto dto)
		{
			try
			{
				Notifications email = new Notifications
				{
					Status = "New",
					Resource = "Contact Form",
					Name = dto.Name,
					Email = dto.Email,
					Message = dto.Message,
					CreatedAt = DateTime.UtcNow
				};
				_dbContext.Notifications.Add(email);
				await _dbContext.SaveChangesAsync();

				await _emailService.SendTicketEmailAsync(dto, new NotificationStatusDto { Status = "New" });

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "[DB Log Error]: Failed to save new notification for {Email}", dto.Email);
				return false;
			}
		}

		public async Task<bool> UpdateNotificationStatusAsync(NotificationStatusDto dto)
		{
			var notification = await _dbContext.Notifications.FindAsync(dto.Id);

			if (notification == null)
			{
				_logger.LogWarning($"Notification with ID {dto.Id} not found. No status update performed.");
				return true;
			}

			try
			{
				notification.Status = dto.Status;
				notification.UpdatedAt = DateTime.UtcNow;

				await _dbContext.SaveChangesAsync();

				var emailFormDto = new EmailFormDto
				{
					Name = notification.Name ?? "",
					Email = notification.Email ?? "",
					Message = notification.Message ?? "",
				};

				var statusUpdateResult = await _emailService.SendTicketEmailAsync(
					emailFormDto,
					new NotificationStatusDto { Status = dto.Status }
				);

				if (!statusUpdateResult)
				{
					_logger.LogWarning($"Failed to send ticket status email for Notification ID {dto.Id}.");
				}

				return true;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"[DB/Service Error] Failed to update notification status for ID {dto.Id}.");
				return false;
			}
		}

		public Task<GetNotificationsDto> GetAllNotificationsAsync()
		{
			var notifications = _dbContext.Notifications
				.OrderByDescending(n => n.Status == "New")
				.ToList();

			if(notifications == null)
			{
				return Task.FromResult(new GetNotificationsDto
				{
					Notifications = new List<Notifications>(),
					NewCount = 0
				});
			}

			var newCount = notifications.Count(n => n.Status == "New");
			var result = new GetNotificationsDto
			{
				Notifications = notifications,
				NewCount = newCount
			};

			return Task.FromResult(result);
			
		}
	}
}

