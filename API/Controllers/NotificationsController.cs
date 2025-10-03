using System.Security.Claims;
using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

/// <summary>
/// Controller for managing notification-related endpoints.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
	INotificationService _notificationService;

	/// <summary>
	/// Initializes a new instance of the <see cref="NotificationsController"/> class.
	/// </summary>
	/// <param name="notificationService">The notification service.</param>
	public NotificationsController(INotificationService notificationService)
	{
		_notificationService = notificationService;
	}

	/// <summary>
	/// Subscribes an admin user to notifications.
	/// </summary>
	/// <param name="subscription">The notification subscription details.</param>
	/// <returns>Returns Ok if successful, Unauthorized if user is not authenticated, or Forbid if not admin.</returns>
	[Authorize]
	[HttpPost("subscribe")]
	public async Task<IActionResult> Subscribe([FromBody] NotificationSubscriptionDto subscription)
	{
		var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		if (userId == null) return Unauthorized();

		var isAdmin = User.IsInRole("Admin");
		if (!isAdmin)
			return Forbid("Only admins can subscribe to notifications.");

		await _notificationService.SaveSubscriptionAsync(subscription, userId);
		return Ok(subscription);
	}

	/// <summary>
	/// Sends a push notification.
	/// </summary>
	/// <param name="dto">The notification message details.</param>
	/// <returns>Returns Ok if successful.</returns>
	[HttpPost("send")]
	public async Task<IActionResult> SendPushNotification([FromBody] NotificationMessageDto dto)
	{
		await _notificationService.SendPushNotificationAsync(dto);
		return Ok();
	}

	/// <summary>
	/// Saves a contact form notification.
	/// </summary>
	/// <param name="dto">The email form details.</param>
	/// <returns>Returns Ok if successful, BadRequest if input is null, or 500 on error.</returns>
	[HttpPost("save-contact-form-notification")]
	public async Task<IActionResult> ContactFormNotification(EmailFormDto dto)
	{
		try
		{
			if (dto == null)
			{
				return BadRequest("Email form can not be null or empty.");
			}

			var result = await _notificationService.SaveEmailNotificationAsync(dto);
			return Ok();
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	/// <summary>
	/// Updates the status of a notification.
	/// </summary>
	/// <param name="dto">The notification status details.</param>
	/// <returns>Returns Ok if successful or 500 on error.</returns>
	[Authorize]
	[HttpPut("update-notification-status")]
	public async Task<IActionResult> UpdateNotificationStatus(NotificationStatusDto dto)
	{
		try
		{
			var result = await _notificationService.UpdateNotificationStatusAsync(dto);
			return Ok();
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}

	/// <summary>
	/// Gets all notifications for the authenticated user.
	/// </summary>
	/// <returns>Returns a list of notifications or 500 on error.</returns>
	[Authorize]
	[HttpGet("all-notifications")]
	public async Task<IActionResult> GetAllNotifications()
	{
		try
		{
			var notifications = await _notificationService.GetAllNotificationsAsync();
			return Ok(notifications);
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal server error: {ex.Message}");
		}
	}
}
