namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Post-startup initialization service. In the original binary this
/// triggered event bus connection and subscription activation.
/// With the RabbitMQ event bus implementation, subscriptions are registered
/// during DI setup (see MyTelegramSessionServerExtensions) and the bus
/// itself starts as a hosted service. This background service logs readiness.
/// </summary>
public sealed class LayeredServiceSubscribeBackgroundService : BackgroundService
{
    private readonly ILogger<LayeredServiceSubscribeBackgroundService> _logger;

    public LayeredServiceSubscribeBackgroundService(
        ILogger<LayeredServiceSubscribeBackgroundService> logger)
    {
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "SessionServer event subscriptions active — " +
            "16 event types registered via DI (AddSubscription<T, SessionEventHandler>)");

        return Task.CompletedTask;
    }
}
