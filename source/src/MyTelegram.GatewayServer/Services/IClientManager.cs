namespace MyTelegram.GatewayServer.Services;

public interface IClientManager
{
    void AddClient(string connectionId,
        ClientData clientData);
    //void UpdateAuthKeyId(long authKeyId, string connectionId);
    void RemoveClient(string connectionId);

    bool TryGetClientData(string connectionId,
        [NotNullWhen(true)] out ClientData? clientData);

    bool TryRemoveClient(string connectionId, [NotNullWhen(true)] out ClientData? clientData);
    int GetOnlineCount();
    bool TryGetClientData(long authKeyId, out ClientData? clientData);
    void UpdateAuthKeyId(ClientData clientData, long authKeyId, string connectionId);
    //bool ContainsAuthKey(long authKeyId);
}