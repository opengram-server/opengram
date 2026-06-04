namespace MyTelegram.GatewayServer.BackgroundServices;

public class ClientDisconnectedDataProcessorBackgroundService(
    IMessageQueueProcessor<ClientDisconnectedEvent> messageQueueProcessor)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return messageQueueProcessor.ProcessAsync(stoppingToken);
    }
}