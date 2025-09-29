using API.Interfaces;
using DomainModels.Dto;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EmailController : ControllerBase   
{
    IEmailService _emailService;

    public EmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost("Sendmail")]
    public async Task<IActionResult> SendEmail(EmailFormDto dto)
    {
        try
        {
            if (dto == null)
            {
                return BadRequest("Email form can not be null or empty.");
            }

            var result = await _emailService.SendEmailNotificationAsync(dto);
            return Ok();
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}
