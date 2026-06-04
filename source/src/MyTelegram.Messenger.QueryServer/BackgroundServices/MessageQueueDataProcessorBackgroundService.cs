using Microsoft.Extensions.Hosting;

namespace MyTelegram.Messenger.QueryServer.BackgroundServices;

public class MessageQueueDataProcessorBackgroundService<TData>(
    IMessageQueueProcessor<TData> processor,
    ILogger<MessageQueueDataProcessorBackgroundService<TData>> logger)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{TypeName} processor started", typeof(TData).Name);
        return processor.ProcessAsync(stoppingToken);
    }
}