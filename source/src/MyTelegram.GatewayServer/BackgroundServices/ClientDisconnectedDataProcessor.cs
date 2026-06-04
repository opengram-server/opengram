namespace MyTelegram.GatewayServer.BackgroundServices;

public class ClientDisconnectedDataProcessor(IEventBus eventBus) : IDataProcessor<ClientDisconnectedEvent>, ITransientDependency
{
    public Task ProcessAsync(ClientDisconnectedEvent data)
    {
        return eventBus.PublishAsync(data);
    }
}