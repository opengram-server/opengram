// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Confirms creation of a secret chat
/// See <a href="https://corefork.telegram.org/method/messages.acceptEncryption" />
///</summary>
internal sealed class AcceptEncryptionHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestAcceptEncryption, MyTelegram.Schema.IEncryptedChat>,
    Messages.IAcceptEncryptionHandler
{
    protected override async Task<MyTelegram.Schema.IEncryptedChat> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestAcceptEncryption obj)
    {
        var chatId = obj.Peer.ChatId;

        // Validate g_b
        if (obj.GB == null || obj.GB.Length == 0)
        {
            RpcErrors.RpcErrors400.EncryptionDeclined.ThrowRpcError();
        }

        var command = new AcceptEncryptionCommand(
            EncryptedChatId.Create(chatId),
            input.ToRequestInfo(),
            obj.GB,
            obj.KeyFingerprint,
            input.PermAuthKeyId);

        await commandBus.PublishAsync(command, default);

        // Return encryptedChat to the acceptor (User B)
        // The DomainEventHandler will push the full update with g_a to User A
        return new TEncryptedChat
        {
            Id = chatId,
            AccessHash = obj.Peer.AccessHash,
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            AdminId = 0, // Will be filled from the update pushed by DomainEventHandler
            ParticipantId = input.UserId,
            GAOrB = ReadOnlyMemory<byte>.Empty, // User B already has g_a from the request
            KeyFingerprint = obj.KeyFingerprint
        };
    }
}
