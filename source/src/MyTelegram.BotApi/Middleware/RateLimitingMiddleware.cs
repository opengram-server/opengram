using System.Collections.Concurrent;

namespace MyTelegram.BotApi.Middleware;

/// <summary>
/// Ограничивает частоту запросов по IP или токену бота, чтобы защититься от DoS-атак.
/// </summary>
public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    
    // Простой in-memory счётчик запросов. В продакшене лучше вынести в Redis.
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _rateLimits = new();

    // Пороги ограничения частоты запросов
    private const int MaxRequestsPerMinute = 120;
    private const int MaxRequestsPerSecond = 10;
    
    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
        
        // Фоновая задача: раз в минуту чистит устаревшие записи
        _ = Task.Run(async () =>
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1));
                CleanupOldEntries();
            }
        });
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var key = GetRateLimitKey(context);
        
        if (!string.IsNullOrEmpty(key))
        {
            var now = DateTimeOffset.UtcNow;
            var info = _rateLimits.GetOrAdd(key, _ => new RateLimitInfo());
            
            bool shouldBlock = false;
            string blockReason = "";
            int retryAfter = 0;
            
            lock (info)
            {
                // Убираем запросы старше одной минуты
                info.Requests.RemoveAll(t => (now - t).TotalMinutes > 1);

                // Проверяем лимит за последнюю секунду
                var recentRequests = info.Requests.Count(t => (now - t).TotalSeconds <= 1);
                if (recentRequests >= MaxRequestsPerSecond)
                {
                    shouldBlock = true;
                    blockReason = "Too Many Requests: Rate limit exceeded (max 10/sec)";
                    retryAfter = 1;
                }
                // Проверяем лимит за минуту
                else if (info.Requests.Count >= MaxRequestsPerMinute)
                {
                    shouldBlock = true;
                    blockReason = "Too Many Requests: Rate limit exceeded (max 120/min)";
                    retryAfter = 60;
                }
                else
                {
                    // Засчитываем текущий запрос
                    info.Requests.Add(now);
                }
            }

            // Блокировку обрабатываем вне lock, чтобы не делать await под блокировкой
            if (shouldBlock)
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("RATE LIMIT: {Reason} - Key: {Key}, IP: {IP}",
                        blockReason, key, context.Connection.RemoteIpAddress);
                }
                
                context.Response.StatusCode = 429;
                context.Response.Headers["Retry-After"] = retryAfter.ToString();
                await context.Response.WriteAsJsonAsync(new
                {
                    ok = false,
                    error_code = 429,
                    description = blockReason
                });
                return;
            }
        }

        await _next(context);
    }
    
    private string GetRateLimitKey(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Для запросов к боту вытаскиваем токен из пути и считаем лимит отдельно по боту
        if (path != null && path.StartsWith("/bot"))
        {
            var match = System.Text.RegularExpressions.Regex.Match(path, @"^/bot([0-9]+):");
            if (match.Success)
            {
                return $"bot:{match.Groups[1].Value}";
            }
        }
        
        // Для остальных запросов лимит считаем по IP-адресу
        var ip = context.Connection.RemoteIpAddress?.ToString();
        return ip != null ? $"ip:{ip}" : string.Empty;
    }
    
    private void CleanupOldEntries()
    {
        var now = DateTimeOffset.UtcNow;
        var keysToRemove = new List<string>();
        
        foreach (var kvp in _rateLimits)
        {
            lock (kvp.Value)
            {
                kvp.Value.Requests.RemoveAll(t => (now - t).TotalMinutes > 2);

                // Если свежих запросов не осталось, запись можно удалить
                if (kvp.Value.Requests.Count == 0)
                {
                    keysToRemove.Add(kvp.Key);
                }
            }
        }
        
        foreach (var key in keysToRemove)
        {
            _rateLimits.TryRemove(key, out _);
        }
        
        if (keysToRemove.Count > 0)
        {
            _logger.LogDebug("Rate limit cleanup: Removed {Count} expired entries", keysToRemove.Count);
        }
    }
    
    private class RateLimitInfo
    {
        public List<DateTimeOffset> Requests { get; } = new();
    }
}
