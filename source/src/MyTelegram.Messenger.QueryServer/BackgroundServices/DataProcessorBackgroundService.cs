using Microsoft.Extensions.Hosting;

namespace MyTelegram.Messenger.QueryServer.BackgroundServices;

public class DataProcessorBackgroundService(
    IMessageQueueProcessor<MessengerQueryDataReceivedEvent> processor,
    ILogger<DataProcessorBackgroundService> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Data processor started");
        return processor.ProcessAsync(stoppingToken);
    }
}