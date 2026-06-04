namespace MyTelegram.GatewayServer.BackgroundServices;

public class UnencryptedDataProcessorBackgroundService(IMessageQueueProcessor<UnencryptedMessage> messageQueueProcessor)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return messageQueueProcessor.ProcessAsync(stoppingToken);
    }
}