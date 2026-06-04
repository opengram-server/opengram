using System.Collections.Concurrent;
using System.Net;

namespace MyTelegram.GatewayServer.Middleware;

/// <summary>
/// Ограничивает число подключений с одного IP.
/// Защищает от DoS-атак за счёт лавины подключений.
/// </summary>
public static class ConnectionLimitMiddleware
{
    private static readonly ConcurrentDictionary<string, int> _connectionsPerIp = new();
    private static readonly Timer _cleanupTimer;
    
    // Параметры конфигурации
    private const int MaxConnectionsPerIp = 100;
    private const int MaxTotalConnections = 10000;
    private static int _totalConnections = 0;
    
    static ConnectionLimitMiddleware()
    {
        // Очистка каждые 5 минут
        _cleanupTimer = new Timer(_ => CleanupStaleEntries(), null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(5));
    }
    
    public static bool CanConnect(IPAddress? ipAddress, out string? errorMessage)
    {
        // Проверяем общее число подключений
        if (_totalConnections >= MaxTotalConnections)
        {
            errorMessage = $"Server at maximum capacity ({MaxTotalConnections} connections)";
            return false;
        }
        
        if (ipAddress == null)
        {
            errorMessage = "Invalid IP address";
            return false;
        }
        
        var ip = ipAddress.ToString();
        
        // С localhost разрешаем неограниченное число подключений (для внутренних сервисов)
        if (IPAddress.IsLoopback(ipAddress))
        {
            Interlocked.Increment(ref _totalConnections);
            errorMessage = null;
            return true;
        }
        
        // Проверяем лимит на один IP
        var currentCount = _connectionsPerIp.GetOrAdd(ip, 0);
        
        if (currentCount >= MaxConnectionsPerIp)
        {
            errorMessage = $"Too many connections from your IP ({currentCount}/{MaxConnectionsPerIp})";
            return false;
        }
        
        _connectionsPerIp.AddOrUpdate(ip, 1, (_, count) => count + 1);
        Interlocked.Increment(ref _totalConnections);
        
        errorMessage = null;
        return true;
    }
    
    public static void OnDisconnect(IPAddress? ipAddress)
    {
        if (ipAddress == null) return;
        
        var ip = ipAddress.ToString();
        
        _connectionsPerIp.AddOrUpdate(ip, 0, (_, count) => Math.Max(0, count - 1));
        Interlocked.Decrement(ref _totalConnections);
        
        // Удаляем запись, если счётчик дошёл до нуля
        if (_connectionsPerIp.TryGetValue(ip, out var count) && count == 0)
        {
            _connectionsPerIp.TryRemove(ip, out _);
        }
    }
    
    private static void CleanupStaleEntries()
    {
        var toRemove = _connectionsPerIp
            .Where(kvp => kvp.Value == 0)
            .Select(kvp => kvp.Key)
            .ToList();
        
        foreach (var ip in toRemove)
        {
            _connectionsPerIp.TryRemove(ip, out _);
        }
        
        if (toRemove.Count > 0)
        {
            Console.WriteLine($"Connection limit cleanup: Removed {toRemove.Count} stale IP entries. Active IPs: {_connectionsPerIp.Count}, Total connections: {_totalConnections}");
        }
    }
    
    public static (int TotalConnections, int UniqueIps) GetStats()
    {
        return (_totalConnections, _connectionsPerIp.Count);
    }
}
