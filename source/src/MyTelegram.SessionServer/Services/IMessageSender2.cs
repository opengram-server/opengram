using MyTelegram.Core;
using MyTelegram.Schema;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Sends RPC results and push notifications to connected clients.
/// Reconstructed from the original binary's MessageSender2.
/// </summary>
public interface IMessageSender2
{
    /// <summary>Send an RPC response back to the requesting client connection.</summary>
    Task<long> SendRpcMessageToClientAsync(
        string connectionId, long reqMsgId, long authKeyId,
        long requestSessionId, IObject data, int seqNumber);

    /// <summary>Send an unsolicited message (update/notification) to a specific connection.</summary>
    Task<long> SendMessageToConnectionAsync(
        string connectionId, long authKeyId, long sessionId,
        IObject data, int seqNumber, int pts, int? qts,
        long globalSeqNo, long reqMsgId);

    /// <summary>Push a layered message to all online sessions of a peer.</summary>
    Task<long> PushMessageToPeerAsync(LayeredPushMessageCreatedIntegrationEvent eventData);

    /// <summary>Send raw pre-serialized data (e.g., RpcResult already serialized by Messenger).</summary>
    Task<long> SendRawDataToClientAsync(
        string connectionId, long reqMsgId, long authKeyId,
        long requestSessionId, ReadOnlyMemory<byte> data, int seqNumber);

    /// <summary>Notify client that auth key is no longer valid.</summary>
    Task SendAuthKeyNotFoundMessageToClientAsync(long authKeyId, string connectionId);

    /// <summary>Process the outbound message queue (called by the background service).</summary>
    Task ProcessOutboundQueueAsync(CancellationToken stoppingToken);
}
