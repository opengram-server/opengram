using MyTelegram.Core;
using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;
using MyTelegram.Services.Services;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's MessageSender2.
/// Routes RPC responses and push notifications to client connections
/// via the internal session data processor (which in turn writes to the
/// network layer via gRPC/pipe).
/// </summary>
public sealed class MessageSender2 : IMessageSender2
{
    private readonly ISessionService _sessionService;
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly IOnlineUserHelper _onlineUserHelper;
    private readonly ILogger<MessageSender2> _logger;

    public MessageSender2(
        ISessionService sessionService,
        IAuthKeyHelper authKeyHelper,
        IOnlineUserHelper onlineUserHelper,
        ILogger<MessageSender2> logger)
    {
        _sessionService = sessionService;
        _authKeyHelper = authKeyHelper;
        _onlineUserHelper = onlineUserHelper;
        _logger = logger;
    }

    public Task<long> SendRpcMessageToClientAsync(
        string connectionId, long reqMsgId, long authKeyId,
        long requestSessionId, IObject data, int seqNumber)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        _logger.LogDebug(
            "SendRpcMessage conn={ConnectionId} reqMsgId={ReqMsgId} authKey={AuthKeyId} msgId={MsgId}",
            connectionId, reqMsgId, authKeyId, msgId);

        // TODO: Wire to actual transport layer (gRPC pipe to GatewayServer)
        // Original flow: serialize IObject → encrypt with auth key → send to connection
        // For now, this is a structural placeholder that will be connected when
        // the GatewayServer transport is implemented.

        return Task.FromResult(msgId);
    }

    public Task<long> SendMessageToConnectionAsync(
        string connectionId, long authKeyId, long sessionId,
        IObject data, int seqNumber, int pts, int? qts,
        long globalSeqNo, long reqMsgId)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        _logger.LogDebug(
            "SendMessage conn={ConnectionId} authKey={AuthKeyId} session={SessionId} pts={Pts}",
            connectionId, authKeyId, sessionId, pts);

        // TODO: Wire to actual transport layer
        return Task.FromResult(msgId);
    }

    public Task<long> PushMessageToPeerAsync(LayeredPushMessageCreatedIntegrationEvent eventData)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        // Original flow:
        // 1. Resolve which users/sessions should receive the push
        // 2. For each online session matching the target peer:
        //    a. Check layer compatibility
        //    b. Serialize appropriately for that layer
        //    c. Send via SendMessageToConnectionAsync
        // 3. Skip sessions belonging to excluded auth keys

        _logger.LogDebug(
            "PushMessageToPeer peerType={PeerType} peerId={PeerId} excludeAuthKeyId={ExcludeAuthKeyId}",
            eventData.PeerType, eventData.PeerId, eventData.ExcludeAuthKeyId);

        // TODO: Implement full push logic with layer resolution
        return Task.FromResult(msgId);
    }

    public Task SendAuthKeyNotFoundMessageToClientAsync(long authKeyId, string connectionId)
    {
        _logger.LogWarning(
            "Auth key not found, notifying client: authKey={AuthKeyId} conn={ConnectionId}",
            authKeyId, connectionId);

        // TODO: Send bad_msg_notification or auth_key_unregistered error to client
        return Task.CompletedTask;
    }

    public async Task ProcessOutboundQueueAsync(CancellationToken stoppingToken)
    {
        // TODO: Implement Channel<T> consumer loop for outbound messages.
        // The original binary maintains an outbound queue that batches messages
        // for efficiency. For now, this is a placeholder that yields control.
        await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
    }
}
