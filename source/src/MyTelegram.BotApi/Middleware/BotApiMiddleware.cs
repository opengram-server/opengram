namespace MyTelegram.BotApi.Middleware;

public class BotApiMiddleware(RequestDelegate next, ILogger<BotApiMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Log all Bot API requests
        if (path?.StartsWith("/bot") == true)
        {
            logger.LogInformation("Bot API Request: {Method} {Path}", context.Request.Method, path);
        }

        await next(context);
    }
}
