using System.Threading.Channels;
using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services.Impl;

/// <summary>
/// Reconstructed from the original binary's SessionDataProcessor.
/// Uses <see cref="Channel{T}"/> for async producer-consumer queue.
/// </summary>
public sealed class SessionDataProcessor : ISessionDataProcessor
{
    private readonly Channel<EncryptedMessage> _queue;

    public SessionDataProcessor()
    {
        _queue = Channel.CreateUnbounded<EncryptedMessage>(new UnboundedChannelOptions
        {
            SingleReader = false,
            SingleWriter = false,
            AllowSynchronousContinuations = false
        });
    }

    public ValueTask EnqueueAsync(EncryptedMessage message)
        => _queue.Writer.WriteAsync(message);

    public ValueTask<EncryptedMessage> DequeueAsync(CancellationToken ct)
        => _queue.Reader.ReadAsync(ct);
}
