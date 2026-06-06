using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Dequeues outgoing push messages and sends them to connected clients.
/// Reconstructed from the original binary's ObjectMessageSenderBackgroundService.
/// This service runs a Channel&lt;T&gt; consumer loop for outbound messages.
/// </summary>
public sealed class ObjectMessageSenderBackgroundService : BackgroundService
{
    private readonly IMessageSender2 _messageSender;
    private readonly ILogger<ObjectMessageSenderBackgroundService> _logger;

    public ObjectMessageSenderBackgroundService(
        IMessageSender2 messageSender,
        ILogger<ObjectMessageSenderBackgroundService> logger)
    {
        _messageSender = messageSender;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ObjectMessageSender background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _messageSender.ProcessOutboundQueueAsync(stoppingToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ObjectMessageSender");
                await Task.Delay(100, stoppingToken).ConfigureAwait(false);
            }
        }

        _logger.LogInformation("ObjectMessageSender background service stopped");
    }
}
