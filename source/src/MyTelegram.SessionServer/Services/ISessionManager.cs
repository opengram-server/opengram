using MyTelegram.SessionServer.Models;

namespace MyTelegram.SessionServer.Services;

public interface ISessionManager
{
    Task<SessionState?> GetSessionAsync(long sessionId);
    Task<SessionState> CreateSessionAsync(long userId, long authKeyId, string platform, string ipAddress);
    Task UpdateSessionAsync(long sessionId, long newSalt, long newMessageId, int newSeqNo);
    Task CloseSessionAsync(long sessionId);
    Task<List<SessionState>> GetUserSessionsAsync(long userId);
    Task<bool> ValidateSessionAsync(long sessionId, long authKeyId);
    Task<AuthKeyInfo?> GetAuthKeyAsync(long authKeyId);
}
