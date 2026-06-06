using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Channel-based queue for processing encrypted messages.
/// Messages are enqueued by the network layer and dequeued by a background service.
/// </summary>
public interface ISessionDataProcessor
{
    ValueTask EnqueueAsync(EncryptedMessage message);
    ValueTask<EncryptedMessage> DequeueAsync(CancellationToken ct);
}
