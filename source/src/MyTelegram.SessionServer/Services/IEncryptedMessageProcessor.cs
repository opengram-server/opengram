using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Main MTProto encrypted message processor.
/// Entry point for all incoming encrypted traffic from clients.
/// </summary>
public interface IEncryptedMessageProcessor
{
    Task ProcessAsync(EncryptedMessage message);
}
