using System.Security.Claims;
using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase   
{
    INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

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

	[HttpPost("send")]
	public async Task<IActionResult> SendPushNotification([FromBody] NotificationMessageDto dto)
	{
		await _notificationService.SendPushNotificationAsync(dto);
		return Ok();
	}

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
