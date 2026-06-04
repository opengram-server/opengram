namespace MyTelegram.BotApi.Middleware;

/// <summary>
/// Ограничивает размер тела запроса, чтобы крупные запросы
/// не приводили к переполнению памяти.
/// </summary>
public class RequestSizeLimitMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestSizeLimitMiddleware> _logger;
    private const long MaxRequestBodySize = 10 * 1024 * 1024; // 10MB

    public RequestSizeLimitMiddleware(RequestDelegate next, ILogger<RequestSizeLimitMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentLength.HasValue)
        {
            if (context.Request.ContentLength.Value > MaxRequestBodySize)
            {
                _logger.LogWarning("Request body too large - Size: {Size}MB, IP: {IP}",
                    context.Request.ContentLength.Value / 1024 / 1024,
                    context.Connection.RemoteIpAddress);

                context.Response.StatusCode = 413; // Payload Too Large (тело запроса слишком большое)
                await context.Response.WriteAsJsonAsync(new
                {
                    ok = false,
                    error_code = 413,
                    description = $"Request body too large (max {MaxRequestBodySize / 1024 / 1024}MB)"
                });
                return;
            }
        }

        // Дополнительно задаём ограничение через feature ASP.NET Core
        var maxRequestBodySizeFeature = context.Features.Get<Microsoft.AspNetCore.Http.Features.IHttpMaxRequestBodySizeFeature>();
        if (maxRequestBodySizeFeature != null && !maxRequestBodySizeFeature.IsReadOnly)
        {
            maxRequestBodySizeFeature.MaxRequestBodySize = MaxRequestBodySize;
        }

        await _next(context);
    }
}
