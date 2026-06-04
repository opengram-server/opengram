namespace MyTelegram.GatewayServer.EventHandlers;

public class EncryptedMessageResponseEventHandler(IClientDataSender clientDataSender)
    : IEventHandler<EncryptedMessageResponse>, ITransientDependency
{
    public Task HandleEventAsync(EncryptedMessageResponse eventData)
    {
        return clientDataSender.SendAsync(eventData);
    }
}