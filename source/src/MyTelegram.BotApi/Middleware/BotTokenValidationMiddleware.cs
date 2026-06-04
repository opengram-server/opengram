using MyTelegram.BotApi.Services;

namespace MyTelegram.BotApi.Middleware;

/// <summary>
/// Проверяет токен бота на всех запросах к Bot API до их обработки.
/// </summary>
public class BotTokenValidationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<BotTokenValidationMiddleware> _logger;

    public BotTokenValidationMiddleware(RequestDelegate next, ILogger<BotTokenValidationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IBotApiService botApiService)
    {
        var path = context.Request.Path.Value;
        
        // Проверяем только пути вида /bot*, пропуская internal, health-check и подобные
        if (path != null && path.StartsWith("/bot") && !path.StartsWith("/botapi"))
        {
            // Достаём токен из пути: /bot{token}/method
            var match = System.Text.RegularExpressions.Regex.Match(path, @"^/bot([0-9]+:[A-Za-z0-9_-]+)/");

            if (match.Success)
            {
                var token = match.Groups[1].Value;

                // Сверяем токен с базой данных
                var isValid = await botApiService.ValidateBotTokenAsync(token);

                if (!isValid)
                {
                    _logger.LogWarning("SECURITY: Invalid bot token rejected - Token: {Token}, IP: {IP}",
                        token, context.Connection.RemoteIpAddress);
                    
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        ok = false,
                        error_code = 401,
                        description = "Unauthorized: Invalid bot token"
                    });
                    return;
                }
                
                _logger.LogDebug("Bot token validated: {Token}", token);
            }
        }

        await _next(context);
    }
}
