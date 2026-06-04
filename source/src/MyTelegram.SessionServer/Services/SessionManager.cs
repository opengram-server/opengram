using System.Text.Json;
using MongoDB.Driver;
using MyTelegram.SessionServer.Models;
using StackExchange.Redis;

namespace MyTelegram.SessionServer.Services;

public class SessionManager : ISessionManager
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IMongoCollection<SessionState> _sessionCollection;
    private readonly IMongoCollection<AuthKeyInfo> _authKeyCollection;
    private readonly ILogger<SessionManager> _logger;
    private readonly IDatabase _redisDb;

    public SessionManager(
        IConnectionMultiplexer redis,
        IMongoDatabase mongoDatabase,
        ILogger<SessionManager> logger)
    {
        _redis = redis;
        _redisDb = redis.GetDatabase();
        _sessionCollection = mongoDatabase.GetCollection<SessionState>("sessions");
        _authKeyCollection = mongoDatabase.GetCollection<AuthKeyInfo>("auth_keys");
        _logger = logger;
    }

    private string GetSessionKey(long sessionId) => $"session:{sessionId}";
    private string GetUserSessionsKey(long userId) => $"user:{userId}:sessions";

    public async Task<SessionState?> GetSessionAsync(long sessionId)
    {
        var key = GetSessionKey(sessionId);
        var cached = await _redisDb.StringGetAsync(key);
        if (cached.HasValue)
        {
            return JsonSerializer.Deserialize<SessionState>(cached.ToString());
        }

        var session = await _sessionCollection.Find(s => s.SessionId == sessionId).FirstOrDefaultAsync();
        if (session != null)
        {
            // Restore to cache
            await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(session), TimeSpan.FromDays(30));
        }

        return session;
    }

    public async Task<SessionState> CreateSessionAsync(long userId, long authKeyId, string platform, string ipAddress)
    {
        // Generate new session ID if it's 0 or collision handling (skipped for simplicity, assuming caller provides or we gen generic)
        // Actually prompt says "Generate sessionId (64-bit)" in "CreateSession" handler. 
        // But ISessionManager interface doesn't take sessionId. Oh, I defined ISessionManager CreateSessionAsync without sessionId in interface too.
        // I'll generate it here.
        var sessionId = Random.Shared.NextInt64();
        var salt = Random.Shared.NextInt64();

        var session = new SessionState
        {
            SessionId = sessionId,
            UserId = userId,
            AuthKeyId = authKeyId,
            Salt = salt,
            State = SessionStateEnum.Active,
            CreatedAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            Platform = platform,
            IpAddress = ipAddress,
            ClientVersion = "1.0", // Default
            SeqNo = 0,
            MessageId = 0
        };

        // 1. Save to MongoDB
        await _sessionCollection.InsertOneAsync(session);

        // 2. Save to Redis
        var key = GetSessionKey(sessionId);
        await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(session), TimeSpan.FromDays(30));
        
        // 3. Add to User Sessions list in Redis
        await _redisDb.SetAddAsync(GetUserSessionsKey(userId), sessionId);

        _logger.LogInformation("Created session {SessionId} for user {UserId}", sessionId, userId);
        return session;
    }

    public async Task UpdateSessionAsync(long sessionId, long newSalt, long newMessageId, int newSeqNo)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return;

        session.Salt = newSalt;
        session.MessageId = newMessageId;
        session.SeqNo = newSeqNo;
        session.LastActivity = DateTime.UtcNow;

        // Update Redis
        var key = GetSessionKey(sessionId);
        await _redisDb.StringSetAsync(key, JsonSerializer.Serialize(session), TimeSpan.FromDays(30));

        // Update MongoDB (Fire and Forget or Async)
        var update = Builders<SessionState>.Update
            .Set(s => s.Salt, newSalt)
            .Set(s => s.MessageId, newMessageId)
            .Set(s => s.SeqNo, newSeqNo)
            .Set(s => s.LastActivity, DateTime.UtcNow);
            
        await _sessionCollection.UpdateOneAsync(s => s.SessionId == sessionId, update);
    }

    public async Task CloseSessionAsync(long sessionId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return;

        // Remove from Redis
        var key = GetSessionKey(sessionId);
        await _redisDb.KeyDeleteAsync(key);
        
        if (session.UserId != 0)
        {
            await _redisDb.SetRemoveAsync(GetUserSessionsKey(session.UserId), sessionId);
        }

        // Update MongoDB
        var update = Builders<SessionState>.Update
            .Set(s => s.State, SessionStateEnum.Closed)
            .Set(s => s.ClosedAt, DateTime.UtcNow);
            
        await _sessionCollection.UpdateOneAsync(s => s.SessionId == sessionId, update);
        _logger.LogInformation("Closed session {SessionId}", sessionId);
    }

    public async Task<List<SessionState>> GetUserSessionsAsync(long userId)
    {
        // Try to get from Redis set first
        var sessionIds = await _redisDb.SetMembersAsync(GetUserSessionsKey(userId));
        var sessions = new List<SessionState>();

        foreach (var idVal in sessionIds)
        {
            if (long.TryParse(idVal.ToString(), out var sid))
            {
                var s = await GetSessionAsync(sid);
                if (s != null && s.State == SessionStateEnum.Active)
                {
                    sessions.Add(s);
                }
            }
        }

        // If Redis empty or partial, maybe sync from Mongo? 
        // For now, if Redis set is empty, we query Mongo.
        if (!sessions.Any())
        {
            sessions = await _sessionCollection.Find(s => s.UserId == userId && s.State == SessionStateEnum.Active).ToListAsync();
        }

        return sessions;
    }

    public async Task<bool> ValidateSessionAsync(long sessionId, long authKeyId)
    {
        var session = await GetSessionAsync(sessionId);
        if (session == null) return false;
        
        if (session.AuthKeyId != authKeyId) return false;
        if (session.State != SessionStateEnum.Active) return false;

        return true;
    }

    public async Task<AuthKeyInfo?> GetAuthKeyAsync(long authKeyId)
    {
        // Check Redis
        // Logic for AuthKeys in redis might be "auth:key:{id}"
        // For implemented scope, let's check Mongo directly if not critical
        // But prompt says "Auth Server" manages this mostly. 
        // Session Server needs it to check validity.
        
        var authKey = await _authKeyCollection.Find(k => k.AuthKeyId == authKeyId).FirstOrDefaultAsync();
        return authKey;
    }
}
