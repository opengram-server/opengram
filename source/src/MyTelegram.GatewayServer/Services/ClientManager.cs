namespace MyTelegram.GatewayServer.Services;

public class ClientManager : IClientManager, ISingletonDependency
{
    private readonly ConcurrentDictionary<string, ClientData> _clients = [];
    private readonly ConcurrentDictionary<long, string> _authKeyIdToConnectionIds = [];

    public void AddClient(string connectionId,
        ClientData clientData)
    {
        _clients.TryAdd(connectionId, clientData);
    }

    public void UpdateAuthKeyId(ClientData clientData, long authKeyId, string connectionId)
    {
        if (clientData.AuthKeyId == 0)
        {
            clientData.AuthKeyId = authKeyId;
            _authKeyIdToConnectionIds.TryRemove(authKeyId, out _);
            _authKeyIdToConnectionIds.TryAdd(authKeyId, connectionId);
        }
    }

    public void RemoveClient(string connectionId)
    {
        if (_clients.TryRemove(connectionId, out var clientData))
        {
            _authKeyIdToConnectionIds.TryRemove(clientData.AuthKeyId, out _);
        }
    }

    public bool TryGetClientData(string connectionId,
        [NotNullWhen(true)] out ClientData? clientData)
    {
        if (_clients.TryGetValue(connectionId, out var d))
        {
            clientData = d;
            return true;
        }

        clientData = null;

        return false;
    }

    public bool TryGetClientData(long authKeyId, out ClientData? clientData)
    {
        if (_authKeyIdToConnectionIds.TryGetValue(authKeyId, out var connectionId))
        {
            return TryGetClientData(connectionId, out clientData);
        }

        clientData = null;

        return false;
    }

    public bool TryRemoveClient(string connectionId, [NotNullWhen(true)] out ClientData? clientData)
    {
        if (_clients.TryRemove(connectionId, out clientData))
        {
            _authKeyIdToConnectionIds.TryRemove(clientData.AuthKeyId, out _);
            return true;
        }

        return false;
    }

    public int GetOnlineCount()
    {
        return _clients.Count;
    }
}