using Microsoft.Extensions.Hosting;

namespace MyTelegram.Messenger.CommandServer.BackgroundServices;

public class ObjectMessageSenderBackgroundService(
    IMessageQueueProcessor<ISessionMessage> processor,
    ILogger<ObjectMessageSenderBackgroundService> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Object message sender service started");
        return processor.ProcessAsync();
    }
}
