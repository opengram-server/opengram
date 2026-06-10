using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Dequeues encrypted messages from the <see cref="ISessionDataProcessor"/>
/// channel and feeds them to the <see cref="IEncryptedMessageProcessor"/>.
/// Reconstructed from the original binary.
/// </summary>
public sealed class SessionDataProcessorBackgroundService : BackgroundService
{
    private readonly ISessionDataProcessor _dataProcessor;
    private readonly IEncryptedMessageProcessor _messageProcessor;
    private readonly ILogger<SessionDataProcessorBackgroundService> _logger;

    public SessionDataProcessorBackgroundService(
        ISessionDataProcessor dataProcessor,
        IEncryptedMessageProcessor messageProcessor,
        ILogger<SessionDataProcessorBackgroundService> logger)
    {
        _dataProcessor = dataProcessor;
        _messageProcessor = messageProcessor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SessionDataProcessor background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var message = await _dataProcessor.DequeueAsync(stoppingToken).ConfigureAwait(false);
                // Fire and forget — individual message failures shouldn't block the queue
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _messageProcessor.ProcessAsync(message).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error processing message from queue");
                    }
                }, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error dequeuing message");
                await Task.Delay(100, stoppingToken).ConfigureAwait(false);
            }
        }

        _logger.LogInformation("SessionDataProcessor background service stopped");
    }
}
