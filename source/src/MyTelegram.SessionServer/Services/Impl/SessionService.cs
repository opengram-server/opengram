using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using MyTelegram.Abstractions;
using MyTelegram.EventBus;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Options;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's SessionService.
/// All sessions are held in-memory (ConcurrentDictionary) keyed by tempAuthKeyId.
/// </summary>
public sealed class SessionService : ISessionService
{
    private readonly ConcurrentDictionary<long, Session> _sessions = new();
    private readonly ILogger<SessionService> _logger;
    private readonly IOnlineUserHelper _onlineUserHelper;
    private readonly IEventBus _eventBus;
    private readonly IOptionsMonitor<MyTelegramSessionServerOptions> _options;

    public SessionService(
        ILogger<SessionService> logger,
        IOnlineUserHelper onlineUserHelper,
        IEventBus eventBus,
        IOptionsMonitor<MyTelegramSessionServerOptions> options)
    {
        _logger = logger;
        _onlineUserHelper = onlineUserHelper;
        _eventBus = eventBus;
        _options = options;
    }

    public Session? GetSession(long authKeyId)
        => _sessions.GetValueOrDefault(authKeyId);

    public bool TryGetSession(long authKeyId, out Session? session)
        => _sessions.TryGetValue(authKeyId, out session);

    public List<Session> GetSessions(long userId)
    {
        var result = new List<Session>();
        foreach (var kvp in _sessions)
        {
            if (kvp.Value.UserId == userId)
                result.Add(kvp.Value);
        }
        return result;
    }

    public long GetUserId(long permAuthKeyId)
    {
        foreach (var kvp in _sessions)
        {
            if (kvp.Value.PermAuthKeyId == permAuthKeyId)
                return kvp.Value.UserId;
        }
        return 0;
    }

    public (Session session, long result) InitConnection(
        string connectionId, long authKeyId, int layer,
        string langPack, string appVersion, uint objectId, long deviceHash)
    {
        var opts = _options.CurrentValue;
        var session = _sessions.GetOrAdd(authKeyId, _ => new Session
        {
            TempAuthKeyId = authKeyId,
            CreateDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TokenBucket = new TokenBucket(opts.RateLimitCapacity, opts.RateLimitTokenRate)
        });

        session.Layer = layer;
        session.DeviceHash = deviceHash;
        session.IsConnectionInitialized = true;

        if (!session.Items.ContainsKey(connectionId))
        {
            session.Items[connectionId] = new SessionItem
            {
                ConnectionId = connectionId,
                ConnectionType = ConnectionType.Generic
            };
        }

        _logger.LogDebug(
            "InitConnection authKey={AuthKeyId} layer={Layer} conn={ConnectionId}",
            authKeyId, layer, connectionId);

        return (session, 0);
    }

    public void DestroySession(string connectionId, long userId, long sessionId)
    {
        foreach (var kvp in _sessions)
        {
            var items = kvp.Value.Items;
            foreach (var item in items)
            {
                if (item.Value.SessionId == sessionId)
                {
                    items.Remove(item.Key);
                    _logger.LogInformation("Destroyed session {SessionId} for connection {ConnectionId}",
                        sessionId, connectionId);
                    return;
                }
            }
        }
    }

    public void BindTempAuthKeyIdToPermAuthKey(long tempAuthKeyId, long permAuthKeyId,
        ConnectionType connectionType)
    {
        var session = _sessions.GetOrAdd(tempAuthKeyId, _ => new Session
        {
            TempAuthKeyId = tempAuthKeyId,
            CreateDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        session.PermAuthKeyId = permAuthKeyId;
        _logger.LogDebug("Bound temp {TempAuthKeyId} to perm {PermAuthKeyId}", tempAuthKeyId, permAuthKeyId);
    }

    public void Deactivate(long permAuthKeyId, bool revoked)
    {
        foreach (var kvp in _sessions)
        {
            if (kvp.Value.PermAuthKeyId == permAuthKeyId)
            {
                kvp.Value.Revoked = revoked;
                kvp.Value.IsOnline = false;
                _logger.LogInformation("Deactivated session for permAuthKeyId={PermAuthKeyId}, revoked={Revoked}",
                    permAuthKeyId, revoked);
            }
        }
    }

    public void SetPushSessionId(long authKeyId, long sessionId)
    {
        if (_sessions.TryGetValue(authKeyId, out var session))
        {
            session.PushSessionId = sessionId;
            session.HasGenericSession = true;
        }
    }

    public Task BindUserIdToSessionAsync(long authKeyId, long userId, long accessHashKeyId)
    {
        if (_sessions.TryGetValue(authKeyId, out var session))
        {
            session.UserId = userId;
            session.AccessHashKeyId = accessHashKeyId;
            _logger.LogDebug("Bound userId={UserId} to authKey={AuthKeyId}", userId, authKeyId);
        }
        return Task.CompletedTask;
    }

    public Task OnUserOfflineAsync(long userId, long tempAuthKeyId)
    {
        if (_sessions.TryGetValue(tempAuthKeyId, out var session))
        {
            session.IsOnline = false;
        }

        _onlineUserHelper.SetOffline(userId, tempAuthKeyId);
        _logger.LogDebug("User {UserId} went offline (authKey={AuthKeyId})", userId, tempAuthKeyId);
        return Task.CompletedTask;
    }

    public async Task OnUserOnlineAsync(string connectionId, long userId, long tempAuthKeyId,
        long permAuthKeyId, bool publishOnlineEvent)
    {
        if (_sessions.TryGetValue(tempAuthKeyId, out var session))
        {
            session.IsOnline = true;
            session.UserId = userId;
        }

        _onlineUserHelper.SetOnline(userId, tempAuthKeyId);

        if (publishOnlineEvent)
        {
            // Publish online status via BindUserIdToSessionEvent so downstream handlers
            // (push notifications, online indicators) can react
            await _eventBus.PublishAsync(new Core.BindUserIdToSessionEvent(userId, tempAuthKeyId, permAuthKeyId))
                .ConfigureAwait(false);
        }

        _logger.LogDebug("User {UserId} came online (authKey={AuthKeyId})", userId, tempAuthKeyId);
    }

    public Task RemoveOfflineClientAsync(string connectionId, long tempAuthKeyId)
    {
        if (_sessions.TryGetValue(tempAuthKeyId, out var session))
        {
            session.Items.Remove(connectionId);
            if (session.Items.Count == 0)
            {
                session.IsOnline = false;
                if (session.UserId != 0)
                    _onlineUserHelper.SetOffline(session.UserId, tempAuthKeyId);
            }
        }
        return Task.CompletedTask;
    }

    public Task<bool> RemoveDisconnectedSessionAsync(string connectionId, long authKeyId, long sessionId)
    {
        if (_sessions.TryGetValue(authKeyId, out var session))
        {
            var removed = session.Items.Remove(connectionId);
            if (session.Items.Count == 0)
            {
                session.IsOnline = false;
                if (session.UserId != 0)
                    _onlineUserHelper.SetOffline(session.UserId, authKeyId);
            }

            _logger.LogDebug("Removed disconnected session conn={ConnectionId} authKey={AuthKeyId}",
                connectionId, authKeyId);
            return Task.FromResult(removed);
        }
        return Task.FromResult(false);
    }
}
