using MyTelegram.SessionServer.Caching;

namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Periodically removes expired temporary auth keys from the in-memory cache.
/// Reconstructed from the original binary's RemoveExpiredAuthKeysBackgroundService.
/// </summary>
public sealed class RemoveExpiredAuthKeysBackgroundService : BackgroundService
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ILogger<RemoveExpiredAuthKeysBackgroundService> _logger;

    public RemoveExpiredAuthKeysBackgroundService(
        IAuthKeyHelper authKeyHelper,
        ILogger<RemoveExpiredAuthKeysBackgroundService> logger)
    {
        _authKeyHelper = authKeyHelper;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("RemoveExpiredAuthKeys background service started");
        await _authKeyHelper.StartRemoveExpiredTempAuthKeyTimerAsync(stoppingToken).ConfigureAwait(false);
        _logger.LogInformation("RemoveExpiredAuthKeys background service stopped");
    }
}
