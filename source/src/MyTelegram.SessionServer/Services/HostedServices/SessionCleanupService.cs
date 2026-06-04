using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.Services.HostedServices;

public class SessionCleanupService : BackgroundService
{
    private readonly ISessionManager _sessionManager;
    private readonly ILogger<SessionCleanupService> _logger;
    private readonly TimeSpan _interval = TimeSpan.FromHours(1);

    public SessionCleanupService(ISessionManager sessionManager, ILogger<SessionCleanupService> logger)
    {
        _sessionManager = sessionManager;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SessionCleanupService started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Cleaning up expired sessions...");
                
                // Logic would call something like _sessionManager.DeleteExpiredSessionsAsync();
                
                await Task.Delay(_interval, stoppingToken);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error occurred executing SessionCleanupService.");
            }
        }
    }
}
