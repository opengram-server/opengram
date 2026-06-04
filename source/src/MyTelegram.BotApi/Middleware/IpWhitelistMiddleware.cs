using System.Net;

namespace MyTelegram.BotApi.Middleware;

/// <summary>
/// Пускает к внутренним эндпоинтам только запросы с доверенных IP-адресов.
/// </summary>
public class IpWhitelistMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IpWhitelistMiddleware> _logger;
    private readonly HashSet<IPAddress> _whitelist;

    public IpWhitelistMiddleware(RequestDelegate next, ILogger<IpWhitelistMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        
        // Читаем список разрешённых адресов из конфигурации, иначе используем значения по умолчанию
        var whitelistConfig = configuration.GetSection("Security:IpWhitelist").Get<string[]>() ?? new[]
        {
            "127.0.0.1",      // localhost
            "::1",            // localhost IPv6
            "172.17.0.0/16",  // Docker default network
            "10.0.0.0/8"      // Internal network
        };
        
        _whitelist = new HashSet<IPAddress>();
        
        foreach (var ip in whitelistConfig)
        {
            if (IPAddress.TryParse(ip, out var addr))
            {
                _whitelist.Add(addr);
                _logger.LogInformation("IP Whitelist: Added {IP}", ip);
            }
        }
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        
        // Проверку по белому списку применяем только к внутренним эндпоинтам
        if (path != null && path.StartsWith("/internal/"))
        {
            var remoteIp = context.Connection.RemoteIpAddress;
            
            if (remoteIp == null || !IsWhitelisted(remoteIp))
            {
                _logger.LogWarning("SECURITY: Internal API access denied - IP: {IP}, Path: {Path}",
                    remoteIp, path);
                
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    error = "Forbidden: Access denied to internal API"
                });
                return;
            }
            
            _logger.LogDebug("Internal API access granted - IP: {IP}", remoteIp);
        }

        await _next(context);
    }

    private bool IsWhitelisted(IPAddress ip)
    {
        // Точное совпадение с адресом из списка
        if (_whitelist.Contains(ip))
            return true;

        // localhost к внутреннему API пускаем всегда
        if (IPAddress.IsLoopback(ip))
            return true;

        // Адреса внутренних и Docker-сетей (172.x.x.x, 10.x.x.x)
        var bytes = ip.GetAddressBytes();
        if (bytes.Length == 4)
        {
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) // 172.16.0.0/12
                return true;
            if (bytes[0] == 10) // 10.0.0.0/8
                return true;
        }
        
        return false;
    }
}
