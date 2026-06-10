using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Manages sessions in-memory via ConcurrentDictionary.
/// Reconstructed from the original binary's SessionService.
/// </summary>
public interface ISessionService
{
    Session? GetSession(long authKeyId);
    bool TryGetSession(long authKeyId, out Session? session);
    List<Session> GetSessions(long userId);
    long GetUserId(long permAuthKeyId);

    (Session session, long result) InitConnection(
        string connectionId, long authKeyId, int layer,
        string langPack, string appVersion, uint objectId, long deviceHash);

    void DestroySession(string connectionId, long userId, long sessionId);
    void BindTempAuthKeyIdToPermAuthKey(long tempAuthKeyId, long permAuthKeyId, ConnectionType connectionType);
    void Deactivate(long permAuthKeyId, bool revoked);
    void SetPushSessionId(long authKeyId, long sessionId);

    Task BindUserIdToSessionAsync(long authKeyId, long userId, long accessHashKeyId);
    Task OnUserOfflineAsync(long userId, long tempAuthKeyId);
    Task OnUserOnlineAsync(string connectionId, long userId, long tempAuthKeyId,
        long permAuthKeyId, bool publishOnlineEvent);
    Task RemoveOfflineClientAsync(string connectionId, long tempAuthKeyId);
    Task<bool> RemoveDisconnectedSessionAsync(string connectionId, long authKeyId, long sessionId);
}
