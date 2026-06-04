namespace MyTelegram.GatewayServer.BackgroundServices;

public class EncryptedDataProcessorBackgroundService(IMessageQueueProcessor<EncryptedMessage> messageQueueProcessor)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return messageQueueProcessor.ProcessAsync(stoppingToken);
    }
}