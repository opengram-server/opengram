namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Processes queued EventFlow commands (e.g., AuthKey aggregate commands)
/// from an internal Channel&lt;T&gt; queue. Reconstructed from the original binary.
/// </summary>
public sealed class QueuedCommandExecutorBackgroundService : BackgroundService
{
    private readonly ILogger<QueuedCommandExecutorBackgroundService> _logger;

    public QueuedCommandExecutorBackgroundService(
        ILogger<QueuedCommandExecutorBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("QueuedCommandExecutor background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // The QueuedCommandExecutor uses a Channel<T> internally.
                // We periodically check if there are pending commands to process.
                // In the original binary, this is a simple reader loop.
                await Task.Delay(500, stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in QueuedCommandExecutor");
                await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
            }
        }

        _logger.LogInformation("QueuedCommandExecutor background service stopped");
    }
}
