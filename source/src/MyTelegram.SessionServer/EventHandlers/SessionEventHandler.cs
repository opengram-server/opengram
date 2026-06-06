using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Services;

namespace MyTelegram.SessionServer.EventHandlers;

/// <summary>
/// Handles integration events published via the EventBus.
/// Reconstructed from the original binary's SessionEventHandler — 16 event subscriptions.
/// Each handler updates in-memory state (sessions, auth keys, online tracking, chat members).
/// </summary>
public sealed class SessionEventHandler :
    IEventHandler<AuthKeyCreatedIntegrationEvent>,
    IEventHandler<BindUserIdToAuthKeyIntegrationEvent>,
    IEventHandler<BindUserIdToAuthKeySuccessEvent>,
    IEventHandler<BindUserIdToSessionEvent>,
    IEventHandler<ClientDisconnectedEvent>,
    IEventHandler<DeviceRegisteredEvent>,
    IEventHandler<SessionPasswordStateChangedEvent>,
    IEventHandler<SetSessionPasswordStateEvent>,
    IEventHandler<UserSignInSuccessEvent>,
    IEventHandler<UnRegisterAuthKeyEvent>,
    IEventHandler<SessionRevokedEvent>,
    IEventHandler<AuthKeyNotFoundEvent>,
    IEventHandler<DataResultResponseReceivedEvent>,
    IEventHandler<LayeredPushMessageCreatedIntegrationEvent>,
    IEventHandler<ChatMemberChangedEvent>,
    IEventHandler<ChannelMemberChangedEvent>
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly IOnlineUserHelper _onlineUserHelper;
    private readonly IChatMemberHelper _chatMemberHelper;
    private readonly IMessageSender2 _messageSender;
    private readonly ILogger<SessionEventHandler> _logger;

    public SessionEventHandler(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        IOnlineUserHelper onlineUserHelper,
        IChatMemberHelper chatMemberHelper,
        IMessageSender2 messageSender,
        ILogger<SessionEventHandler> logger)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _onlineUserHelper = onlineUserHelper;
        _chatMemberHelper = chatMemberHelper;
        _messageSender = messageSender;
        _logger = logger;
    }

    /// <summary>A new auth key was created by the AuthServer.</summary>
    public Task HandleEventAsync(AuthKeyCreatedIntegrationEvent e)
    {
        var authKeyId = System.Buffers.Binary.BinaryPrimitives.ReadInt64LittleEndian(
            e.Data.Span.Length >= 8 ? e.Data.Span : stackalloc byte[8]);

        _authKeyHelper.SetAuthKeyItem(authKeyId, new AuthKeyItem
        {
            PermAuthKeyId = e.IsPermanent ? authKeyId : 0,
            TempAuthKeyId = e.IsPermanent ? 0 : authKeyId,
            Data = e.Data,
            ServerSalt = e.ServerSalt,
            IsActive = true
        });

        _logger.LogInformation("Auth key created: authKeyId={AuthKeyId} isPerm={IsPermanent}",
            authKeyId, e.IsPermanent);
        return Task.CompletedTask;
    }

    /// <summary>Messenger tells us to bind a userId to an auth key.</summary>
    public Task HandleEventAsync(BindUserIdToAuthKeyIntegrationEvent e)
    {
        _authKeyHelper.UpdateUserId(e.AuthKeyId, e.UserId);
        _authKeyHelper.UpdateUserId(e.PermAuthKeyId, e.UserId);

        _logger.LogDebug("BindUserIdToAuthKey: userId={UserId} authKey={AuthKeyId} perm={PermAuthKeyId}",
            e.UserId, e.AuthKeyId, e.PermAuthKeyId);
        return Task.CompletedTask;
    }

    /// <summary>Binding succeeded — update session too.</summary>
    public Task HandleEventAsync(BindUserIdToAuthKeySuccessEvent e)
    {
        _authKeyHelper.UpdateUserId(e.TempAuthKeyId, e.UserId);
        _authKeyHelper.UpdateUserId(e.PermAuthKeyId, e.UserId);

        return _sessionService.BindUserIdToSessionAsync(e.TempAuthKeyId, e.UserId, 0);
    }

    /// <summary>User session binding — update online status.</summary>
    public async Task HandleEventAsync(BindUserIdToSessionEvent e)
    {
        await _sessionService.BindUserIdToSessionAsync(e.AuthKeyId, e.UserId, 0).ConfigureAwait(false);
        _onlineUserHelper.SetOnline(e.UserId, e.AuthKeyId);

        _logger.LogDebug("BindUserIdToSession: userId={UserId} authKey={AuthKeyId}", e.UserId, e.AuthKeyId);
    }

    /// <summary>Client TCP/WebSocket connection dropped.</summary>
    public async Task HandleEventAsync(ClientDisconnectedEvent e)
    {
        await _sessionService.RemoveDisconnectedSessionAsync(e.ConnectionId, e.AuthKeyId, e.SessionId)
            .ConfigureAwait(false);

        _logger.LogDebug("Client disconnected: conn={ConnectionId} authKey={AuthKeyId}",
            e.ConnectionId, e.AuthKeyId);
    }

    /// <summary>Device registered for push notifications.</summary>
    public Task HandleEventAsync(DeviceRegisteredEvent e)
    {
        _sessionService.SetPushSessionId(e.AuthKeyId, e.SessionId);

        _logger.LogDebug("Device registered: authKey={AuthKeyId} session={SessionId}",
            e.AuthKeyId, e.SessionId);
        return Task.CompletedTask;
    }

    /// <summary>Password state changed for a specific auth key.</summary>
    public Task HandleEventAsync(SessionPasswordStateChangedEvent e)
    {
        var session = _sessionService.GetSession(e.AuthKeyId);
        if (session != null)
        {
            session.PasswordState = e.PasswordState;
        }

        _logger.LogDebug("Password state changed: authKey={AuthKeyId} state={State}",
            e.AuthKeyId, e.PasswordState);
        return Task.CompletedTask;
    }

    /// <summary>Set password state for all sessions of a user.</summary>
    public Task HandleEventAsync(SetSessionPasswordStateEvent e)
    {
        var sessions = _sessionService.GetSessions(e.UserId);
        foreach (var session in sessions)
        {
            session.PasswordState = e.PasswordState;
        }

        _logger.LogDebug("Set password state for userId={UserId} state={State} ({Count} sessions)",
            e.UserId, e.PasswordState, sessions.Count);
        return Task.CompletedTask;
    }

    /// <summary>User successfully signed in.</summary>
    public async Task HandleEventAsync(UserSignInSuccessEvent e)
    {
        _authKeyHelper.UpdateUserId(e.TempAuthKeyId, e.UserId);
        _authKeyHelper.UpdateUserId(e.PermAuthKeyId, e.UserId);

        await _sessionService.BindUserIdToSessionAsync(e.TempAuthKeyId, e.UserId, 0).ConfigureAwait(false);

        var session = _sessionService.GetSession(e.TempAuthKeyId);
        if (session != null)
        {
            session.PasswordState = e.PasswordState;
        }

        _logger.LogInformation("User sign-in success: userId={UserId} tempAuth={TempAuthKeyId}",
            e.UserId, e.TempAuthKeyId);
    }

    /// <summary>Auth key unregistered (user logged out from another session).</summary>
    public async Task HandleEventAsync(UnRegisterAuthKeyEvent e)
    {
        _sessionService.Deactivate(e.PermAuthKeyId, revoked: false);
        _authKeyHelper.RemoveAuthKeyItem(e.PermAuthKeyId);

        if (e.UserId != 0)
        {
            var authKeys = _onlineUserHelper.GetOnlineAuthKeyIds(e.UserId);
            foreach (var ak in authKeys)
            {
                var item = _authKeyHelper.GetAuthKeyItem(ak);
                if (item?.PermAuthKeyId == e.PermAuthKeyId)
                {
                    _onlineUserHelper.SetOffline(e.UserId, ak);
                }
            }
        }

        _logger.LogInformation("Auth key unregistered: permAuthKey={PermAuthKeyId} userId={UserId}",
            e.PermAuthKeyId, e.UserId);
    }

    /// <summary>Sessions revoked (auth.resetAuthorizations).</summary>
    public Task HandleEventAsync(SessionRevokedEvent e)
    {
        foreach (var revokedPermAuthKeyId in e.RevokedPermAuthKeyIdList)
        {
            _sessionService.Deactivate(revokedPermAuthKeyId, revoked: true);
        }

        _logger.LogInformation("Sessions revoked for userId={UserId}, count={Count}",
            e.UserId, e.RevokedPermAuthKeyIdList.Count);
        return Task.CompletedTask;
    }

    /// <summary>Auth key was not found during request processing.</summary>
    public Task HandleEventAsync(AuthKeyNotFoundEvent e)
    {
        return _messageSender.SendAuthKeyNotFoundMessageToClientAsync(e.AuthKeyId, e.ConnectionId);
    }

    /// <summary>RPC result received from the Messenger server — forward to client.</summary>
    public async Task HandleEventAsync(DataResultResponseReceivedEvent e)
    {
        // The result needs to be routed back to the originating client connection.
        // This is a simplified implementation — the original binary tracks reqMsgId→connectionId
        // mapping to route responses correctly.
        _logger.LogDebug("DataResultResponse received: reqMsgId={ReqMsgId}", e.ReqMsgId);

        // TODO: Look up the connection info for this reqMsgId and forward the result.
        // The mapping is maintained by the EncryptedMessageProcessor when dispatching.
    }

    /// <summary>Push notification to be sent to all online sessions of a peer.</summary>
    public async Task HandleEventAsync(LayeredPushMessageCreatedIntegrationEvent e)
    {
        await _messageSender.PushMessageToPeerAsync(e).ConfigureAwait(false);
    }

    /// <summary>Chat members changed — update the cached member list.</summary>
    public Task HandleEventAsync(ChatMemberChangedEvent e)
    {
        UpdateMemberCache(e.ChatId, e.MemberStateChangeType, e.MemberUidList.ToList());
        return Task.CompletedTask;
    }

    /// <summary>Channel members changed — update the cached member list.</summary>
    public Task HandleEventAsync(ChannelMemberChangedEvent e)
    {
        UpdateMemberCache(e.ChannelId, e.MemberStateChangeType, e.MemberUidList.ToList());
        return Task.CompletedTask;
    }

    private void UpdateMemberCache(long chatId, MemberStateChangeType changeType, List<long> memberIds)
    {
        switch (changeType)
        {
            case MemberStateChangeType.Added:
                foreach (var uid in memberIds)
                    _chatMemberHelper.AddMember(chatId, uid);
                break;
            case MemberStateChangeType.Removed:
                foreach (var uid in memberIds)
                    _chatMemberHelper.RemoveMember(chatId, uid);
                break;
            default:
                _chatMemberHelper.SetMembers(chatId, memberIds);
                break;
        }
    }
}
