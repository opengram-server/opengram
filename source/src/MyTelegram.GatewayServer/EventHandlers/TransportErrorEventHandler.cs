namespace MyTelegram.GatewayServer.EventHandlers;

public class TransportErrorEventHandler(IClientDataSender clientDataSender)
    : IEventHandler<TransportErrorEvent>, ITransientDependency
{
    public Task HandleEventAsync(TransportErrorEvent eventData)
    {
        var m = new EncryptedMessageResponse(eventData.AuthKeyId, BitConverter.GetBytes(eventData.TransportErrorCode),
            eventData.ConnectionId, 2);
        return clientDataSender.SendAsync(m);
    }
}