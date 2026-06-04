namespace MyTelegram.GatewayServer.EventHandlers;

public class UnencryptedMessageResponseEventHandler(IClientDataSender clientDataSender)
    : IEventHandler<UnencryptedMessageResponse>, ITransientDependency
{
    public Task HandleEventAsync(UnencryptedMessageResponse eventData)
    {
        return clientDataSender.SendAsync(eventData);
    }
}