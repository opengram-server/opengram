namespace MyTelegram.GatewayServer.EventHandlers;

public class UnRegisterAuthKeyEventHandler(IClientManager clientManager, ILogger<UnRegisterAuthKeyEventHandler> logger)
    : IEventHandler<UnRegisterAuthKeyEvent>, ITransientDependency
{
    public async Task HandleEventAsync(UnRegisterAuthKeyEvent eventData)
    {
        logger.LogInformation("Handling UnRegisterAuthKeyEvent for authKeyId: {AuthKeyId}, userId: {UserId}", 
            eventData.PermAuthKeyId, eventData.UserId);

        if (clientManager.TryGetClientData(eventData.PermAuthKeyId, out var clientData))
        {
            logger.LogInformation("Disconnecting client with connectionId: {ConnectionId}, authKeyId: {AuthKeyId}", 
                clientData.ConnectionId, eventData.PermAuthKeyId);

            // Close the connection immediately
            if (clientData.ConnectionContext != null)
            {
                await clientData.ConnectionContext.DisposeAsync();
            }

            if (clientData.WebSocket != null)
            {
                await clientData.WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
                    "Authorization revoked", CancellationToken.None);
            }

            clientManager.RemoveClient(clientData.ConnectionId);
            
            logger.LogInformation("Client disconnected successfully for authKeyId: {AuthKeyId}", eventData.PermAuthKeyId);
        }
        else
        {
            logger.LogWarning("Cannot find client with authKeyId: {AuthKeyId}", eventData.PermAuthKeyId);
        }
    }
}
