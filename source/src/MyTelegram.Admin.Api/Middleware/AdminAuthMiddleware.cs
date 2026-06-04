using System.Security.Claims;

namespace MyTelegram.Admin.Api.Middleware;

/// <summary>
/// Middleware аутентификации для Admin API.
/// Для всех админских эндпоинтов требует API-ключ или токен.
/// </summary>
public class AdminAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminAuthMiddleware> _logger;
    private readonly string _adminApiKey;

    public AdminAuthMiddleware(RequestDelegate next, ILogger<AdminAuthMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        // Берём ключ из конфигурации или переменной окружения,
        // значение по умолчанию пригодно только для разработки
        _adminApiKey = configuration["ADMIN_API_KEY"]
            ?? Environment.GetEnvironmentVariable("ADMIN_API_KEY")
            ?? "CHANGE_ME_IN_PRODUCTION";

        if (_adminApiKey == "CHANGE_ME_IN_PRODUCTION")
        {
            _logger.LogWarning("Using default Admin API key! Set ADMIN_API_KEY environment variable.");
        }
        else
        {
            _logger.LogInformation("Admin API authentication enabled with custom key");
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Проверяем аутентификацию для всех эндпоинтов /api/*
        if (path != null && path.StartsWith("/api/"))
        {
            // Сначала ищем ключ в заголовке X-Admin-API-Key
            if (!context.Request.Headers.TryGetValue("X-Admin-API-Key", out var apiKey) ||
                apiKey != _adminApiKey)
            {
                // Затем пробуем заголовок Authorization с Bearer-токеном
                if (!context.Request.Headers.TryGetValue("Authorization", out var authHeader) ||
                    !authHeader.ToString().StartsWith("Bearer ") ||
                    authHeader.ToString().Substring(7) != _adminApiKey)
                {
                    _logger.LogWarning("Unauthorized Admin API access attempt - IP: {IP}, Path: {Path}",
                        context.Connection.RemoteIpAddress, path);
                    
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(new
                    {
                        success = false,
                        error = "Unauthorized: Invalid or missing API key"
                    });
                    return;
                }
            }
            
            _logger.LogDebug("Admin API access authorized - Path: {Path}", path);
        }

        await _next(context);
    }
}
