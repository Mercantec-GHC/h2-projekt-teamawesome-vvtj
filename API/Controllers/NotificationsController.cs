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
	public async Task<IActionResult> Send([FromBody] NotificationMessageDto dto)
	{
		await _notificationService.SendNotificationAsync(dto);
		return Ok();
	}
}
