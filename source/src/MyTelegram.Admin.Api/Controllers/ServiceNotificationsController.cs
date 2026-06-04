using Microsoft.AspNetCore.Mvc;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Admin.Api.Controllers;

[ApiController]
[Route("api/service-notifications")]
public class ServiceNotificationsController : ControllerBase
{
    private readonly IServiceNotificationAppService _serviceNotificationAppService;
    private readonly ILogger<ServiceNotificationsController> _logger;

    public ServiceNotificationsController(
        IServiceNotificationAppService serviceNotificationAppService,
        ILogger<ServiceNotificationsController> logger)
    {
        _serviceNotificationAppService = serviceNotificationAppService;
        _logger = logger;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest request)
    {
        _logger.LogInformation("Received notification request for user {UserId}: Type={Type}, Popup={Popup}",
            request.UserId, request.Type, request.Popup);

        try
        {
            await _serviceNotificationAppService.SendServiceNotificationAsync(
                request.UserId,
                request.Type,
                request.Message,
                request.Popup,
                media: null, // TODO: добавить поддержку медиа
                entities: null,
                invertMedia: false
            );

            _logger.LogInformation("Notification sent successfully to user {UserId}", request.UserId);

            return Ok(new { success = true, message = "Notification sent" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send notification to user {UserId}", request.UserId);
            return StatusCode(500, new { success = false, error = ex.Message });
        }
    }
}

public class SendNotificationRequest
{
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Popup { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
}
