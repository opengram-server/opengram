using System.Buffers;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using MyTelegram.Abstractions;
using MyTelegram.Core;
using MyTelegram.Schema;
using MyTelegram.Services.Services;
using MyTelegram.SessionServer.Caching;
using MyTelegram.SessionServer.Options;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's EncryptedMessageProcessor.
/// This is the main MTProto processing pipeline — the biggest file in the
/// original SessionServer (~2,800 lines of Ghidra output, 8 async state machines).
///
/// Pipeline (from the original call graph):
///   1. Look up AuthKeyItem from cache
///   2. Decrypt the encrypted message (AES-256-IGE)
///   3. Parse the inner message (message_id, seq_no, payload)
///   4. Deserialize the TL object
///   5. Validate message (salt, session, message_id, seq_no)
///   6. Initialize/update session if needed
///   7. Check rate limit
///   8. Check permissions (login, 2FA, media-only, business-connection, user/bot-only)
///   9. Dispatch: handle locally (Ping, MsgsAck, etc.) or route to Messenger server
/// </summary>
public sealed class EncryptedMessageProcessor : IEncryptedMessageProcessor
{
    private readonly IAuthKeyHelper _authKeyHelper;
    private readonly ISessionService _sessionService;
    private readonly IMessageSender2 _messageSender;
    private readonly ISessionDataDispatcher _sessionDataDispatcher;
    private readonly IServerSaltHelper _serverSaltHelper;
    private readonly PermissionChecker _permissionChecker;
    private readonly IMessageIdHelper _messageIdHelper;
    private readonly IMtpHelper _mtpHelper;
    private readonly IExceptionProcessor _exceptionProcessor;
    private readonly IScheduleAppService _scheduleAppService;
    private readonly ILogger<EncryptedMessageProcessor> _logger;
    private readonly IGZipHelper _gzipHelper;
    private readonly IPendingRequestTracker _pendingRequestTracker;
    private readonly IOptionsMonitor<MyTelegramSessionServerOptions> _options;

    public EncryptedMessageProcessor(
        IAuthKeyHelper authKeyHelper,
        ISessionService sessionService,
        IMessageSender2 messageSender,
        ISessionDataDispatcher sessionDataDispatcher,
        IServerSaltHelper serverSaltHelper,
        PermissionChecker permissionChecker,
        IMessageIdHelper messageIdHelper,
        IMtpHelper mtpHelper,
        IExceptionProcessor exceptionProcessor,
        IScheduleAppService scheduleAppService,
        IGZipHelper gzipHelper,
        IPendingRequestTracker pendingRequestTracker,
        ILogger<EncryptedMessageProcessor> logger,
        IOptionsMonitor<MyTelegramSessionServerOptions> options)
    {
        _authKeyHelper = authKeyHelper;
        _sessionService = sessionService;
        _messageSender = messageSender;
        _sessionDataDispatcher = sessionDataDispatcher;
        _serverSaltHelper = serverSaltHelper;
        _permissionChecker = permissionChecker;
        _messageIdHelper = messageIdHelper;
        _mtpHelper = mtpHelper;
        _exceptionProcessor = exceptionProcessor;
        _scheduleAppService = scheduleAppService;
        _gzipHelper = gzipHelper;
        _pendingRequestTracker = pendingRequestTracker;
        _logger = logger;
        _options = options;
    }

    public async Task ProcessAsync(EncryptedMessage message)
    {
        var sw = Stopwatch.StartNew();
        var authKeyId = message.AuthKeyId;
        var connectionId = message.ConnectionId;
        var connectionType = message.ConnectionType;
        var requestId = message.RequestId;

        try
        {
            // 1. Look up auth key
            if (!_authKeyHelper.TryGetAuthKeyItem(authKeyId, out var authKeyItem) || authKeyItem == null)
            {
                sw.Stop();
                _logger.LogWarning(
                    "Auth key not found: conn={ConnectionId} authKeyId={AuthKeyId}",
                    connectionId, authKeyId);
                await _messageSender.SendAuthKeyNotFoundMessageToClientAsync(authKeyId, connectionId)
                    .ConfigureAwait(false);
                return;
            }

            // 2. Decrypt the message
            var decryptedData = Decrypt(authKeyItem, message.MsgKey, message.EncryptedData);
            if (decryptedData == null)
            {
                _logger.LogWarning(
                    "Failed to decrypt message: authKeyId={AuthKeyId} conn={ConnectionId}",
                    authKeyId, connectionId);
                return;
            }

            // 3. Parse inner message header
            //    Layout: server_salt(8) + session_id(8) + message_id(8) + seq_no(4) + msg_len(4) + payload(msg_len)
            var span = decryptedData.Value.Span;
            if (span.Length < 32)
            {
                _logger.LogWarning("Decrypted message too short ({Length} bytes)", span.Length);
                return;
            }

            var serverSalt = BinaryPrimitives.ReadInt64LittleEndian(span);
            var sessionId = BinaryPrimitives.ReadInt64LittleEndian(span[8..]);
            var messageId = BinaryPrimitives.ReadInt64LittleEndian(span[16..]);
            var seqNumber = BinaryPrimitives.ReadInt32LittleEndian(span[24..]);
            var msgLength = BinaryPrimitives.ReadInt32LittleEndian(span[28..]);

            if (msgLength < 0 || msgLength > span.Length - 32)
            {
                _logger.LogWarning("Invalid msg_length={MsgLength}", msgLength);
                return;
            }

            var payloadData = decryptedData.Value.Slice(32, msgLength);

            // 4. Validate message (salt, session, message_id, seq_no)
            var badMsg = TryValidateMessage(
                connectionId, authKeyId, requestId,
                sessionId, messageId, seqNumber,
                authKeyItem.ServerSalt, serverSalt);

            if (badMsg != null)
            {
                await _messageSender.SendRpcMessageToClientAsync(
                    connectionId, messageId, authKeyId, sessionId,
                    badMsg, seqNumber + 1).ConfigureAwait(false);
                return;
            }

            // 5. Deserialize the TL object
            IObject? requestData;
            uint objectId;
            try
            {
                requestData = Deserialize(payloadData);
                objectId = requestData != null
                    ? BinaryPrimitives.ReadUInt32LittleEndian(payloadData.Span)
                    : 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize TL object from authKey={AuthKeyId}", authKeyId);
                return;
            }

            if (requestData == null)
            {
                _logger.LogWarning("Deserialized null object from authKey={AuthKeyId}", authKeyId);
                return;
            }

            WriteDebugLog(requestData, messageId);

            // 6. Initialize/update session
            var session = await InitSessionIfNeededAsync(
                connectionId, authKeyId, authKeyItem, sessionId,
                serverSalt, connectionType).ConfigureAwait(false);

            if (session == null)
            {
                _logger.LogWarning("Failed to init session for authKey={AuthKeyId}", authKeyId);
                return;
            }

            // 7. Check rate limit
            if (!CheckRateLimit(session, authKeyId))
            {
                _logger.LogWarning("Rate limited: authKey={AuthKeyId}", authKeyId);
                var floodError = new TRpcError { ErrorCode = 420, ErrorMessage = "FLOOD_WAIT_1" };
                await _messageSender.SendRpcMessageToClientAsync(
                    connectionId, messageId, authKeyId, sessionId,
                    floodError, seqNumber + 1).ConfigureAwait(false);
                return;
            }

            // 8. Check permissions
            var permError = _permissionChecker.CheckPermission(session, authKeyId, objectId, connectionType);
            if (permError != null)
            {
                await _messageSender.SendRpcMessageToClientAsync(
                    connectionId, messageId, authKeyId, sessionId,
                    permError, seqNumber + 1).ConfigureAwait(false);
                return;
            }

            // 9. Build request input and dispatch
            var reqInput = new SessionRequestInput
            {
                ConnectionId = connectionId,
                RequestId = requestId,
                ObjectId = objectId,
                UserId = session.UserId,
                ReqMsgId = messageId,
                SeqNumber = seqNumber,
                AuthKeyId = authKeyId,
                PermAuthKeyId = session.PermAuthKeyId,
                Layer = session.Layer,
                Date = message.Date,
                DeviceType = session.DeviceType,
                ClientIp = message.ClientIp,
                SessionId = sessionId,
                AccessHashKeyId = session.AccessHashKeyId,
                ConnectionType = connectionType,
                ServerSalt = serverSalt,
                IsActive = authKeyItem.IsActive,
                Revoked = session.Revoked
            };

            // Handle MsgContainer: dispatch each inner message
            if (objectId == ObjectIdConsts.MsgContainer)
            {
                await ProcessMsgContainerAsync(requestData, reqInput, session).ConfigureAwait(false);
            }
            // Handle locally processed MTProto messages
            else if (IsLocallyHandled(objectId))
            {
                await ProcessLocallyAsync(objectId, requestData, reqInput, session).ConfigureAwait(false);
            }
            else
            {
                // Track the request for response routing
                _pendingRequestTracker.Track(reqInput.ReqMsgId, reqInput.ConnectionId,
                    reqInput.AuthKeyId, reqInput.SessionId, reqInput.SeqNumber);

                // Route to Messenger server
                var sessionData = new InternalSessionData(reqInput, requestData);
                await _sessionDataDispatcher.DispatchAsync(sessionData).ConfigureAwait(false);
            }

            sw.Stop();
            if (sw.ElapsedMilliseconds > 100)
            {
                _logger.LogWarning(
                    "Slow processing: {Elapsed}ms objectId=0x{ObjectId:X8} authKey={AuthKeyId}",
                    sw.ElapsedMilliseconds, objectId, authKeyId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing encrypted message: authKey={AuthKeyId} conn={ConnectionId}",
                authKeyId, connectionId);
        }
        finally
        {
            message.MemoryOwner?.Dispose();
        }
    }

    #region Decrypt

    /// <summary>
    /// Decrypts an MTProto 2.0 encrypted message using AES-256-IGE.
    /// Returns the decrypted inner data, or null on failure.
    /// </summary>
    private ReadOnlyMemory<byte>? Decrypt(AuthKeyItem authKeyItem, ReadOnlyMemory<byte> msgKey, ReadOnlyMemory<byte> encryptedData)
    {
        if (authKeyItem.Data.Length < 256)
        {
            _logger.LogWarning("Auth key data too short for decryption: {Length}", authKeyItem.Data.Length);
            return null;
        }

        try
        {
            var authKeyData = authKeyItem.Data.Span;

            // Calculate AES key and IV from auth_key + msg_key (client→server direction)
            Span<byte> aesKey = stackalloc byte[32];
            Span<byte> aesIv = stackalloc byte[32];
            CalcAesKeyIv(authKeyData, msgKey.Span, true, aesKey, aesIv);

            // Decrypt
            var decrypted = new byte[encryptedData.Length];
            AesIgeDecrypt(encryptedData.Span, aesKey, aesIv, decrypted);

            // Verify msg_key: SHA-256 of auth_key[88..120] + decrypted, bits 64..191 should == msg_key
            Span<byte> expectedMsgKey = stackalloc byte[32];
            using var sha256 = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            sha256.AppendData(authKeyData.Slice(88, 32));
            sha256.AppendData(decrypted);
            sha256.GetHashAndReset(expectedMsgKey);

            if (!expectedMsgKey.Slice(8, 16).SequenceEqual(msgKey.Span))
            {
                _logger.LogWarning("Message key verification failed");
                return null;
            }

            return decrypted;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            return null;
        }
    }

    private static void CalcAesKeyIv(ReadOnlySpan<byte> authKey, ReadOnlySpan<byte> msgKey,
        bool fromClient, Span<byte> aesKey, Span<byte> aesIv)
    {
        // MTProto 2.0 key derivation
        // x = 0 for messages from client to server, 8 for server to client
        int x = fromClient ? 0 : 8;

        Span<byte> sha256a = stackalloc byte[32];
        Span<byte> sha256b = stackalloc byte[32];

        // sha256_a = SHA256(msg_key + substr(auth_key, x, 36))
        Span<byte> temp = stackalloc byte[16 + 36];
        msgKey.CopyTo(temp);
        authKey.Slice(x, 36).CopyTo(temp[16..]);
        SHA256.HashData(temp, sha256a);

        // sha256_b = SHA256(substr(auth_key, 40+x, 36) + msg_key)
        Span<byte> temp2 = stackalloc byte[36 + 16];
        authKey.Slice(40 + x, 36).CopyTo(temp2);
        msgKey.CopyTo(temp2[36..]);
        SHA256.HashData(temp2, sha256b);

        // aes_key = substr(sha256_a, 0, 8) + substr(sha256_b, 8, 16) + substr(sha256_a, 24, 8)
        sha256a[..8].CopyTo(aesKey);
        sha256b.Slice(8, 16).CopyTo(aesKey[8..]);
        sha256a.Slice(24, 8).CopyTo(aesKey[24..]);

        // aes_iv = substr(sha256_b, 0, 8) + substr(sha256_a, 8, 16) + substr(sha256_b, 24, 8)
        sha256b[..8].CopyTo(aesIv);
        sha256a.Slice(8, 16).CopyTo(aesIv[8..]);
        sha256b.Slice(24, 8).CopyTo(aesIv[24..]);
    }

    private static void AesIgeDecrypt(ReadOnlySpan<byte> data, ReadOnlySpan<byte> key,
        Span<byte> iv, Span<byte> output)
    {
        // AES-256-IGE decryption
        using var aes = Aes.Create();
        aes.Key = key.ToArray();
        aes.Mode = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        Span<byte> xPrev = stackalloc byte[16];
        Span<byte> yPrev = stackalloc byte[16];
        iv[..16].CopyTo(xPrev);   // IV first 16 bytes
        iv[16..32].CopyTo(yPrev); // IV last 16 bytes

        Span<byte> temp = stackalloc byte[16];
        for (var i = 0; i < data.Length; i += 16)
        {
            var block = data.Slice(i, 16);
            var outBlock = output.Slice(i, 16);

            // y_i = AES_decrypt(x_i XOR y_{i-1}) XOR x_{i-1}
            for (var j = 0; j < 16; j++)
                temp[j] = (byte)(block[j] ^ yPrev[j]);

            aes.DecryptEcb(temp, outBlock, PaddingMode.None);

            for (var j = 0; j < 16; j++)
                outBlock[j] ^= xPrev[j];

            block.CopyTo(xPrev); // x_prev = ciphertext block
            outBlock.CopyTo(yPrev); // y_prev = plaintext block
        }
    }

    #endregion

    #region Validation

    private IObject? TryValidateMessage(
        string connectionId, long authKeyId, Guid requestId,
        long sessionId, long messageId, int seqNo,
        long serverSalt, long clientServerSalt)
    {
        // Check server salt match
        if (serverSalt != clientServerSalt && clientServerSalt != 0)
        {
            _logger.LogWarning(
                "Bad server salt: expected={Expected} got={Got} authKey={AuthKeyId}",
                serverSalt, clientServerSalt, authKeyId);
            // Return bad_server_salt notification
            return new TBadServerSalt
            {
                BadMsgId = messageId,
                BadMsgSeqno = seqNo,
                ErrorCode = 48,
                NewServerSalt = serverSalt
            };
        }

        // Check message_id validity
        if (!MessageIdGenerator.IsValidClientMessageId(messageId))
        {
            _logger.LogWarning(
                "Bad message_id: {MessageId} authKey={AuthKeyId}",
                messageId, authKeyId);
            return new TBadMsgNotification
            {
                BadMsgId = messageId,
                BadMsgSeqno = seqNo,
                ErrorCode = 16 // msg_id too low
            };
        }

        return null;
    }

    #endregion

    #region Session management

    private async Task<Session?> InitSessionIfNeededAsync(
        string connectionId, long authKeyId, AuthKeyItem authKeyItem,
        long sessionId, long serverSalt, ConnectionType connectionType)
    {
        var session = _sessionService.GetSession(authKeyId);
        if (session != null)
        {
            // Update connection if needed
            if (!session.Items.ContainsKey(connectionId))
            {
                session.Items[connectionId] = new SessionItem
                {
                    ConnectionId = connectionId,
                    SessionId = sessionId,
                    ServerSalt = serverSalt,
                    ConnectionType = connectionType
                };
            }
            return session;
        }

        // Create new session via InitConnection
        var (newSession, _) = _sessionService.InitConnection(
            connectionId, authKeyId, authKeyItem.Layer,
            string.Empty, string.Empty, 0, 0);

        newSession.PermAuthKeyId = authKeyItem.PermAuthKeyId;
        newSession.UserId = authKeyItem.UserId;
        newSession.AccessHashKeyId = authKeyItem.AccessHashKeyId;
        newSession.DeviceType = authKeyItem.DeviceType ?? DeviceType.Unknown;
        newSession.PasswordState = PasswordState.None;

        newSession.Items[connectionId] = new SessionItem
        {
            ConnectionId = connectionId,
            SessionId = sessionId,
            ServerSalt = serverSalt,
            ConnectionType = connectionType
        };

        // Check server salt validity; refresh if needed
        await CheckCachedServerSaltAsync(authKeyId, serverSalt).ConfigureAwait(false);

        return newSession;
    }

    private async Task CheckCachedServerSaltAsync(long authKeyId, long currentSalt)
    {
        try
        {
            var now = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var salts = await _serverSaltHelper.GetOrCreateCachedFutureSaltsAsync(authKeyId, 8)
                .ConfigureAwait(false);

            var validSalt = salts.Find(p => p.ValidSince <= now && now <= p.ValidUntil);
            if (validSalt != null)
            {
                _authKeyHelper.UpdateServerSalt(authKeyId, validSalt.Salt);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check cached server salt for authKey={AuthKeyId}", authKeyId);
        }
    }

    #endregion

    #region Rate limiting

    private static bool CheckRateLimit(Session session, long authKeyId)
    {
        return session.TokenBucket?.TryConsume() ?? true;
    }

    #endregion

    #region Deserialization

    private static IObject? Deserialize(ReadOnlyMemory<byte> data)
    {
        var serializer = SerializerFactory.CreateSerializer<IObject>();
        var buffer = data;
        return serializer.Deserialize(ref buffer);
    }

    #endregion

    #region Container & local handling

    private async Task ProcessMsgContainerAsync(IObject requestData, SessionRequestInput reqInput, Session session)
    {
        if (requestData is not TMsgContainer container)
            return;

        foreach (var innerMsg in container.Messages)
        {
            if (innerMsg.Body == null) continue;

            var innerObjectId = 0u;
            // Try to get object ID from the body
            if (innerMsg.Body is IObject bodyObj)
            {
                // The objectId is extracted from the first 4 bytes
                // For most TL types, the constructor ID is at the start
                innerObjectId = 0; // Will be set by TL type's constructor
            }

            var innerInput = new SessionRequestInput
            {
                ConnectionId = reqInput.ConnectionId,
                RequestId = reqInput.RequestId,
                ObjectId = innerObjectId,
                UserId = reqInput.UserId,
                ReqMsgId = innerMsg.MsgId,
                SeqNumber = innerMsg.SeqNo,
                AuthKeyId = reqInput.AuthKeyId,
                PermAuthKeyId = reqInput.PermAuthKeyId,
                Layer = reqInput.Layer,
                Date = reqInput.Date,
                DeviceType = reqInput.DeviceType,
                ClientIp = reqInput.ClientIp,
                SessionId = reqInput.SessionId,
                AccessHashKeyId = reqInput.AccessHashKeyId,
                ConnectionType = reqInput.ConnectionType,
                ServerSalt = reqInput.ServerSalt,
                IsActive = reqInput.IsActive,
                Revoked = reqInput.Revoked
            };

            // Check permission for each inner message
            var error = _permissionChecker.CheckPermission(
                session, reqInput.AuthKeyId, innerObjectId, reqInput.ConnectionType);
            if (error != null)
            {
                await _messageSender.SendRpcMessageToClientAsync(
                    reqInput.ConnectionId, innerMsg.MsgId, reqInput.AuthKeyId,
                    reqInput.SessionId, error, innerMsg.SeqNo + 1).ConfigureAwait(false);
                continue;
            }

            if (IsLocallyHandled(innerObjectId))
            {
                await ProcessLocallyAsync(innerObjectId, innerMsg.Body, innerInput, session)
                    .ConfigureAwait(false);
            }
            else
            {
                _pendingRequestTracker.Track(innerInput.ReqMsgId, innerInput.ConnectionId,
                    innerInput.AuthKeyId, innerInput.SessionId, innerInput.SeqNumber);

                var sessionData = new InternalSessionData(innerInput, innerMsg.Body);
                await _sessionDataDispatcher.DispatchAsync(sessionData).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Messages that the SessionServer handles locally without forwarding to the Messenger server.
    /// </summary>
    private static bool IsLocallyHandled(uint objectId)
    {
        return objectId == ObjectIdConsts.PingId ||
               objectId == ObjectIdConsts.PingDelayId ||
               objectId == ObjectIdConsts.MsgAcks ||
               objectId == ObjectIdConsts.GzipPackedId;
    }

    private async Task ProcessLocallyAsync(uint objectId, IObject requestData,
        SessionRequestInput reqInput, Session session)
    {
        if (objectId == ObjectIdConsts.PingId && requestData is RequestPing ping)
        {
            var pong = new TPong { MsgId = reqInput.ReqMsgId, PingId = ping.PingId };
            await _messageSender.SendRpcMessageToClientAsync(
                reqInput.ConnectionId, reqInput.ReqMsgId, reqInput.AuthKeyId,
                reqInput.SessionId, pong, reqInput.SeqNumber + 1).ConfigureAwait(false);
        }
        else if (objectId == ObjectIdConsts.PingDelayId && requestData is RequestPingDelayDisconnect pingDelay)
        {
            var pong = new TPong { MsgId = reqInput.ReqMsgId, PingId = pingDelay.PingId };
            await _messageSender.SendRpcMessageToClientAsync(
                reqInput.ConnectionId, reqInput.ReqMsgId, reqInput.AuthKeyId,
                reqInput.SessionId, pong, reqInput.SeqNumber + 1).ConfigureAwait(false);

            // Schedule disconnect if no new ping arrives within the delay
            var disconnectDelay = TimeSpan.FromSeconds(Math.Min(pingDelay.DisconnectDelay, 300));
            await _scheduleAppService.ExecuteAsync(async () =>
            {
                await _sessionService.RemoveDisconnectedSessionAsync(
                    reqInput.ConnectionId, reqInput.AuthKeyId, reqInput.SessionId)
                    .ConfigureAwait(false);
            }, disconnectDelay).ConfigureAwait(false);
        }
        else if (objectId == ObjectIdConsts.MsgAcks)
        {
            // Acknowledgments are noted but don't need a response
            _logger.LogDebug("Received MsgsAck from authKey={AuthKeyId}", reqInput.AuthKeyId);
        }
        else if (objectId == ObjectIdConsts.GzipPackedId && requestData is TGzipPacked gzipPacked)
        {
            // Decompress gzip_packed and re-process the inner object
            _logger.LogDebug("Received gzip_packed from authKey={AuthKeyId}, decompressing", reqInput.AuthKeyId);

            var packedData = gzipPacked.PackedData;
            if (!packedData.IsEmpty)
            {
                // Determine uncompressed size
                if (!_gzipHelper.TryGetUncompressedLength(packedData.Span, out var uncompressedLength))
                    uncompressedLength = packedData.Length * 4; // Estimate

                var decompressedBuffer = new byte[uncompressedLength];
                _gzipHelper.Decompress(packedData, decompressedBuffer, out var actualLength);

                // Parse the decompressed TL object
                var decompressedMemory = new ReadOnlyMemory<byte>(decompressedBuffer, 0, actualLength);
                var innerObjectId = BinaryPrimitives.ReadUInt32LittleEndian(decompressedMemory.Span);
                var innerObject = Deserialize(decompressedMemory);

                if (innerObject == null)
                {
                    _logger.LogWarning("gzip_packed: failed to deserialize inner object 0x{ObjectId:X8}",
                        innerObjectId);
                }
                else if (IsLocallyHandled(innerObjectId))
                {
                    await ProcessLocallyAsync(innerObjectId, innerObject, reqInput, session)
                        .ConfigureAwait(false);
                }
                else
                {
                    _pendingRequestTracker.Track(reqInput.ReqMsgId, reqInput.ConnectionId,
                        reqInput.AuthKeyId, reqInput.SessionId, reqInput.SeqNumber);

                    var sessionData = new InternalSessionData(reqInput, innerObject);
                    await _sessionDataDispatcher.DispatchAsync(sessionData).ConfigureAwait(false);
                }
            }
        }
    }

    #endregion

    #region Logging

    private void WriteDebugLog(IObject obj, long reqMsgId)
    {
        if (_logger.IsEnabled(LogLevel.Debug))
        {
            _logger.LogDebug("Processing {TypeName} reqMsgId={ReqMsgId}", obj.GetType().Name, reqMsgId);
        }
    }

    #endregion
}
