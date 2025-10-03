using System.Text.Json;
using API.Data;
using API.Interfaces;
using DomainModels.Dto;
using DomainModels.Models;
using Microsoft.EntityFrameworkCore;
using WebPush;

namespace API.Services
{
	/// <summary>
	/// Service for handling notifications, including push and email notifications.
	/// </summary>
	public class NotificationService : INotificationService
	{
		private readonly AppDBContext _dbContext;
		private readonly ILogger<NotificationService> _logger;
		private readonly IConfiguration _config;
		private readonly IEmailService _emailService;

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationService"/> class.
		/// </summary>
		public NotificationService(AppDBContext context, ILogger<NotificationService> logger, IConfiguration config, IEmailService emailService)
		{
			_dbContext = context;
			_logger = logger;
			_config = config;
			_emailService = emailService;
		}

		/// <summary>
		/// Saves a push notification subscription for a user, replacing any previous subscriptions.
		/// </summary>
		/// <param name="subscription">The subscription DTO.</param>
		/// <param name="userId">The user's ID.</param>
		public async Task SaveSubscriptionAsync(NotificationSubscriptionDto subscription, string userId)
		{
			// Remove old subscriptions for the user to ensure only one active subscription per user.
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

		/// <summary>
		/// Sends a push notification to all registered subscriptions.
		/// </summary>
		/// <param name="dto">The notification message DTO.</param>
		public async Task SendPushNotificationAsync(NotificationMessageDto dto)
		{
			try
			{
				var subscriptions = await _dbContext.NotificationSubscriptions.ToListAsync();

				// Retrieve VAPID keys from configuration for WebPush authentication.
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

		/// <summary>
		/// Saves an email notification and sends a ticket email.
		/// </summary>
		/// <param name="dto">The email form DTO.</param>
		/// <returns>True if successful, otherwise false.</returns>
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

		/// <summary>
		/// Updates the status of a notification and sends a status update email.
		/// </summary>
		/// <param name="dto">The notification status DTO.</param>
		/// <returns>True if successful, otherwise false.</returns>
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
				notification.Status = dto.Status ?? "New";
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

		/// <summary>
		/// Gets all notifications, ordered by status, and counts new notifications.
		/// </summary>
		/// <returns>A DTO containing the notifications and the count of new notifications.</returns>
		public Task<GetNotificationsDto> GetAllNotificationsAsync()
		{
			// Order notifications so that "New" status appears first.
			var notifications = _dbContext.Notifications
				.OrderByDescending(n => n.Status == "New")
				.ToList();

			if (notifications == null)
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

