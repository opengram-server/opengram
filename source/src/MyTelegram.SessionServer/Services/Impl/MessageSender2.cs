using System.Buffers;
using System.Buffers.Binary;
using MyTelegram.Abstractions;
using MyTelegram.Core;
using MyTelegram.EventBus;
using MyTelegram.Schema;
using MyTelegram.SessionServer.Caching;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's MessageSender2.
/// Routes RPC responses and push notifications to client connections by:
///   1. Looking up the auth key data from the AuthKeyHelper cache
///   2. Serializing the TL object into a MTProto inner data frame
///      (server_salt + session_id + message_id + seq_no + message_data_length + data + padding)
///   3. Encrypting via IMtpHelper.Encrypt (AES-256-IGE, MTProto 2.0)
///   4. Publishing EncryptedMessageResponse via EventBus → GatewayServer sends to TCP client
/// </summary>
public sealed class MessageSender2 : IMessageSender2
{
    private readonly ISessionService _sessionService;
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly IOnlineUserHelper _onlineUserHelper;
    private readonly IChatMemberHelper _chatMemberHelper;
    private readonly IMtpHelper _mtpHelper;
    private readonly IEventBus _eventBus;
    private readonly ILogger<MessageSender2> _logger;

    public MessageSender2(
        ISessionService sessionService,
        IAuthKeyHelper authKeyHelper,
        IOnlineUserHelper onlineUserHelper,
        IChatMemberHelper chatMemberHelper,
        IMtpHelper mtpHelper,
        IEventBus eventBus,
        ILogger<MessageSender2> logger)
    {
        _sessionService = sessionService;
        _authKeyHelper = authKeyHelper;
        _onlineUserHelper = onlineUserHelper;
        _chatMemberHelper = chatMemberHelper;
        _mtpHelper = mtpHelper;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<long> SendRpcMessageToClientAsync(
        string connectionId, long reqMsgId, long authKeyId,
        long requestSessionId, IObject data, int seqNumber)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        var authKeyItem = _authKeyHelper.GetAuthKeyItem(authKeyId);
        if (authKeyItem == null)
        {
            _logger.LogWarning("SendRpcMessage: auth key not found for {AuthKeyId}", authKeyId);
            return 0;
        }

        // Wrap in TRpcResult
        var rpcResult = new TRpcResult { ReqMsgId = reqMsgId, Result = data };

        await SendMessageToConnectionCoreAsync(
            connectionId, authKeyId, requestSessionId,
            rpcResult, ReadOnlyMemory<byte>.Empty, seqNumber, authKeyItem,
            msgId, 0, null, 0, reqMsgId).ConfigureAwait(false);

        return msgId;
    }

    public async Task<long> SendRawDataToClientAsync(
        string connectionId, long reqMsgId, long authKeyId,
        long requestSessionId, ReadOnlyMemory<byte> data, int seqNumber)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        var authKeyItem = _authKeyHelper.GetAuthKeyItem(authKeyId);
        if (authKeyItem == null)
        {
            _logger.LogWarning("SendRawData: auth key not found for {AuthKeyId}", authKeyId);
            return 0;
        }

        await SendMessageToConnectionCoreAsync(
            connectionId, authKeyId, requestSessionId,
            null, data, seqNumber, authKeyItem,
            msgId, 0, null, 0, reqMsgId).ConfigureAwait(false);

        return msgId;
    }

    public async Task<long> SendMessageToConnectionAsync(
        string connectionId, long authKeyId, long sessionId,
        IObject data, int seqNumber, int pts, int? qts,
        long globalSeqNo, long reqMsgId)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        var authKeyItem = _authKeyHelper.GetAuthKeyItem(authKeyId);
        if (authKeyItem == null)
        {
            _logger.LogWarning("SendMessage: auth key not found for {AuthKeyId}", authKeyId);
            return 0;
        }

        await SendMessageToConnectionCoreAsync(
            connectionId, authKeyId, sessionId,
            data, ReadOnlyMemory<byte>.Empty, seqNumber, authKeyItem,
            msgId, pts, qts, globalSeqNo, reqMsgId).ConfigureAwait(false);

        return msgId;
    }

    public async Task<long> PushMessageToPeerAsync(LayeredPushMessageCreatedIntegrationEvent eventData)
    {
        var msgId = MessageIdGenerator.GenerateServerMessageId();

        // Resolve target users based on peer type
        IReadOnlyList<long> targetUserIds;
        switch (eventData.PeerType)
        {
            case PeerType.User:
            case PeerType.Self:
                targetUserIds = [eventData.PeerId];
                break;
            case PeerType.Chat:
            case PeerType.Channel:
                var members = _chatMemberHelper.GetMembers(eventData.PeerId);
                if (members == null || members.Count == 0)
                {
                    _logger.LogDebug(
                        "PushMessage: no members found for peer {PeerType}/{PeerId}",
                        eventData.PeerType, eventData.PeerId);
                    return msgId;
                }
                targetUserIds = members;
                break;
            default:
                _logger.LogWarning("PushMessage: unknown peer type {PeerType}", eventData.PeerType);
                return msgId;
        }

        foreach (var userId in targetUserIds)
        {
            // Skip excluded user
            if (eventData.ExcludeUserId.HasValue && userId == eventData.ExcludeUserId.Value)
                continue;

            // Only send to online users
            if (!_onlineUserHelper.IsOnline(userId))
                continue;

            await PushMessageToOnlineUserAsync(eventData, userId).ConfigureAwait(false);
        }

        return msgId;
    }

    private async Task PushMessageToOnlineUserAsync(
        LayeredPushMessageCreatedIntegrationEvent eventData, long userId)
    {
        var sessions = _sessionService.GetSessions(userId);
        if (sessions.Count == 0)
            return;

        foreach (var session in sessions)
        {
            // Skip excluded auth keys
            if (eventData.ExcludeAuthKeyId.HasValue)
            {
                var excluded = eventData.ExcludeAuthKeyId.Value;
                if (session.TempAuthKeyId == excluded || session.PermAuthKeyId == excluded)
                    continue;
            }

            // Skip if only sending to a specific auth key
            if (eventData.OnlySendToThisAuthKeyId.HasValue)
            {
                var target = eventData.OnlySendToThisAuthKeyId.Value;
                if (session.TempAuthKeyId != target && session.PermAuthKeyId != target)
                    continue;
            }

            var authKeyItem = _authKeyHelper.GetAuthKeyItem(session.TempAuthKeyId);
            if (authKeyItem == null)
                continue;

            // Send to all connection items within this session
            foreach (var item in session.Items.Values)
            {
                var msgId = MessageIdGenerator.GenerateServerMessageId();

                try
                {
                    await SendMessageToConnectionCoreAsync(
                        item.ConnectionId, session.TempAuthKeyId, item.SessionId,
                        null, eventData.Data, 0, authKeyItem,
                        msgId, eventData.Pts, eventData.Qts,
                        eventData.GlobalSeqNo, 0).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "PushMessage failed for user={UserId} authKey={AuthKeyId} conn={ConnectionId}",
                        userId, session.TempAuthKeyId, item.ConnectionId);
                }
            }
        }
    }

    public async Task SendAuthKeyNotFoundMessageToClientAsync(long authKeyId, string connectionId)
    {
        _logger.LogWarning(
            "Auth key not found, notifying client: authKey={AuthKeyId} conn={ConnectionId}",
            authKeyId, connectionId);

        // Publish AuthKeyNotFoundEvent — GatewayServer will send transport error to client
        await _eventBus.PublishAsync(new AuthKeyNotFoundEvent(authKeyId, connectionId))
            .ConfigureAwait(false);
    }

    public async Task ProcessOutboundQueueAsync(CancellationToken stoppingToken)
    {
        // The outbound queue is processed inline in the current architecture.
        // Each SendMessageToConnectionCoreAsync call publishes directly to the event bus.
        // This method exists for the background service pattern — yield control periodically.
        await Task.Delay(1000, stoppingToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Core method: builds MTProto inner data, encrypts with auth key, publishes to EventBus.
    /// Reconstructed from the original binary's SendMessageToConnectionCoreAsync state machine.
    /// Flow: serialize → build inner data frame → encrypt (AES-256-IGE) → publish EncryptedMessageResponse
    /// </summary>
    private async Task SendMessageToConnectionCoreAsync(
        string connectionId, long authKeyId, long sessionId,
        IObject? dataObject, ReadOnlyMemory<byte> rawData, int seqNumber,
        AuthKeyItem authKeyItem, long msgId,
        int pts, int? qts, long globalSeqNo, long reqMsgId)
    {
        // Step 1: Serialize the TL object if not already raw bytes
        ReadOnlyMemory<byte> serializedData;
        if (!rawData.IsEmpty)
        {
            serializedData = rawData;
        }
        else if (dataObject != null)
        {
            using var writer = new ArrayPoolBufferWriter<byte>();
            dataObject.Serialize(writer);
            serializedData = writer.WrittenMemory.ToArray();
        }
        else
        {
            _logger.LogWarning("SendMessageCore: no data to send for authKey={AuthKeyId}", authKeyId);
            return;
        }

        // Step 2: Build MTProto inner data frame
        // Layout: server_salt(8) + session_id(8) + message_id(8) + seq_no(4) + data_length(4) + data + padding
        var dataLength = serializedData.Length;
        var innerDataLength = 8 + 8 + 8 + 4 + 4 + dataLength; // 32 + dataLength
        // Pad to 16-byte boundary (at least 12 bytes of random padding per MTProto 2.0)
        var paddingLength = 16 - (innerDataLength % 16);
        if (paddingLength < 12)
            paddingLength += 16;
        var totalInnerLength = innerDataLength + paddingLength;

        var innerData = ArrayPool<byte>.Shared.Rent(totalInnerLength);
        try
        {
            var span = innerData.AsSpan(0, totalInnerLength);

            // server_salt (8 bytes)
            BinaryPrimitives.WriteInt64LittleEndian(span, authKeyItem.ServerSalt);
            // session_id (8 bytes)
            BinaryPrimitives.WriteInt64LittleEndian(span.Slice(8), sessionId);
            // message_id (8 bytes)
            BinaryPrimitives.WriteInt64LittleEndian(span.Slice(16), msgId);
            // seq_no (4 bytes)
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(24), seqNumber);
            // message_data_length (4 bytes)
            BinaryPrimitives.WriteInt32LittleEndian(span.Slice(28), dataLength);
            // message_data
            serializedData.Span.CopyTo(span.Slice(32));
            // random padding
            Random.Shared.NextBytes(span.Slice(innerDataLength, paddingLength));

            // Step 3: Encrypt with MTProto 2.0
            // Output: auth_key_id(8) + msg_key(16) + encrypted_data
            var encryptedLength = 24 + totalInnerLength;
            var encryptedBuffer = ArrayPool<byte>.Shared.Rent(encryptedLength);
            try
            {
                var authKeyData = authKeyItem.Data.ToArray();
                _mtpHelper.Encrypt(authKeyId, authKeyData,
                    span.Slice(0, totalInnerLength),
                    encryptedBuffer.AsSpan(0, encryptedLength));

                // Step 4: Publish EncryptedMessageResponse via EventBus
                // GatewayServer subscribes to this event and sends to the TCP client
                var encryptedData = new byte[encryptedLength];
                encryptedBuffer.AsSpan(0, encryptedLength).CopyTo(encryptedData);

                await _eventBus.PublishAsync(new EncryptedMessageResponse(
                    authKeyId, encryptedData, connectionId, seqNumber)).ConfigureAwait(false);

                _logger.LogDebug(
                    "Sent encrypted message: authKey={AuthKeyId} conn={ConnectionId} msgId={MsgId} size={Size}",
                    authKeyId, connectionId, msgId, encryptedLength);

                // Step 5: Publish PTS/QTS update events if applicable
                // These signal the Messenger service that an update has been delivered
                if (pts > 0)
                {
                    // The PTS ack is handled by the state tracking in the domain layer,
                    // not by the session server directly.
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(encryptedBuffer);
            }
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(innerData);
        }
    }
}
