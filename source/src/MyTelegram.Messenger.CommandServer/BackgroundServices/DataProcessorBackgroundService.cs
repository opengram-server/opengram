using Microsoft.Extensions.Hosting;

namespace MyTelegram.Messenger.CommandServer.BackgroundServices;

public class DataProcessorBackgroundService(
    IMessageQueueProcessor<MessengerCommandDataReceivedEvent> processor,
    ILogger<DataProcessorBackgroundService> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Data processor started");
        return processor.ProcessAsync();
    }
}