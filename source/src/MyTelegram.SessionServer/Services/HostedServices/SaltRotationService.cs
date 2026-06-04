using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Services.HostedServices;

public class SaltRotationService : BackgroundService
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<SaltRotationService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(30);

    public SaltRotationService(ISessionManager sessionManager, ILogger<SaltRotationService> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SaltRotationService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // In a real implementation we would stream active sessions or use a specialized query.
                // Since this is a sample/skeleton, we'll log iteration.
                // To actually implement, we'd need a method in manager to "RotateAllSalts()" 
                // possibly utilizing a Lua script in Redis or a bulk update in Mongo.
                
                _logger.LogInformation("Rotating salts for active sessions...");
                
                // Placeholder/Simulation logic
                // var activeSessions = await _sessionManager.GetActiveSessions(); 
                // foreach(var session in activeSessions) { ... }
                
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error occurred executing SaltRotationService.");
            }
        }
    }
}
