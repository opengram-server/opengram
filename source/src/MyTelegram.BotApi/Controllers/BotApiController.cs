using Microsoft.AspNetCore.Mvc;
using MyTelegram.BotApi.Services;
using System.Text.Json;

namespace MyTelegram.BotApi.Controllers;

[ApiController]
public class BotApiController(
    IBotApiService botApiService,
    ILogger<BotApiController> logger) : ControllerBase
{
    // Проверяет токен бота. Возвращает null, если токен верный, иначе ответ с ошибкой.
    private async Task<IActionResult?> ValidateTokenAsync(string botId, string secretToken)
    {
        var token = $"{botId}:{secretToken}";

        if (!await botApiService.ValidateBotTokenAsync(token))
        {
            logger.LogWarning("SECURITY: Invalid bot token attempted - BotId: {BotId}", botId);
            return Unauthorized(new { ok = false, error_code = 401, description = "Unauthorized: Invalid bot token" });
        }

        return null; // токен корректен
    }

    [HttpGet("bot{botId}:{secretToken}/getMe")]
    public async Task<IActionResult> GetMe(string botId, string secretToken)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetMeAsync(token);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getMe");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" }); // не раскрываем детали ошибки наружу
        }
    }

    [HttpGet("bot{botId}:{secretToken}/getUpdates")]
    public async Task<IActionResult> GetUpdates(
        string botId,
        string secretToken,
        [FromQuery] int offset = 0,
        [FromQuery] int limit = 100,
        [FromQuery] int timeout = 0)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetUpdatesAsync(token, offset, limit, timeout);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getUpdates");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" }); // не раскрываем детали ошибки наружу
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendMessage")]
    public async Task<IActionResult> SendMessage(string botId, string secretToken, [FromBody] JsonElement body)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendMessageAsync(token, body);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendMessage");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/setWebhook")]
    public async Task<IActionResult> SetWebhook(string botId, string secretToken, [FromBody] JsonElement body)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SetWebhookAsync(token, body);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in setWebhook");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" }); // не раскрываем детали ошибки наружу
        }
    }

    [HttpPost("bot{botId}:{secretToken}/deleteWebhook")]
    public async Task<IActionResult> DeleteWebhook(string botId, string secretToken)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.DeleteWebhookAsync(token);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in deleteWebhook");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" }); // не раскрываем детали ошибки наружу
        }
    }

    [HttpGet("bot{botId}:{secretToken}/getAvailableGifts")]
    public async Task<IActionResult> GetAvailableGifts(string botId, string secretToken)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetAvailableGiftsAsync(token);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getAvailableGifts");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendGift")]
    public async Task<IActionResult> SendGift(string botId, string secretToken, [FromBody] JsonElement body)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendGiftAsync(token, body);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendGift");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }

    [HttpGet("bot{botId}:{secretToken}/getWebhookInfo")]
    public async Task<IActionResult> GetWebhookInfo(string botId, string secretToken)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.GetWebhookInfoAsync(token);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in getWebhookInfo");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/sendInvoice")]
    public async Task<IActionResult> SendInvoice(string botId, string secretToken, [FromBody] JsonElement body)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.SendInvoiceAsync(token, body);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in sendInvoice");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }

    [HttpPost("bot{botId}:{secretToken}/answerPreCheckoutQuery")]
    public async Task<IActionResult> AnswerPreCheckoutQuery(string botId, string secretToken, [FromBody] JsonElement body)
    {
        // Проверяем токен бота
        var authError = await ValidateTokenAsync(botId, secretToken);
        if (authError != null) return authError;
        
        try
        {
            var token = $"{botId}:{secretToken}";
            var result = await botApiService.AnswerPreCheckoutQueryAsync(token, body);
            return Ok(new { ok = true, result });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in answerPreCheckoutQuery");
            return Ok(new { ok = false, error_code = 400, description = "Bad Request" });
        }
    }
}
