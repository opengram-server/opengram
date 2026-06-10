// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Sends a request to start a secret chat to the user.
/// See <a href="https://corefork.telegram.org/method/messages.requestEncryption" />
///</summary>
internal sealed class RequestEncryptionHandler(
    ICommandBus commandBus,
    IAccessHashHelper accessHashHelper,
    IPeerHelper peerHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestRequestEncryption, MyTelegram.Schema.IEncryptedChat>,
    Messages.IRequestEncryptionHandler
{
    protected override async Task<MyTelegram.Schema.IEncryptedChat> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestRequestEncryption obj)
    {
        // Validate target user
        await accessHashHelper.CheckAccessHashAsync(input, obj.UserId);
        var toPeer = peerHelper.GetPeer(obj.UserId, input.UserId);
        var toUserId = toPeer.PeerId;

        if (toUserId == input.UserId)
        {
            RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
        }

        // Validate g_a
        if (obj.GA == null || obj.GA.Length == 0)
        {
            RpcErrors.RpcErrors400.DhGAInvalid.ThrowRpcError();
        }

        // Use RandomId from client as chatId (per Telegram protocol)
        var chatId = obj.RandomId;
        var accessHash = Random.Shared.NextInt64();
        var date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var command = new RequestEncryptionCommand(
            EncryptedChatId.Create(chatId),
            input.ToRequestInfo(),
            chatId,
            accessHash,
            input.UserId,
            toUserId,
            input.PermAuthKeyId,
            obj.GA,
            date);

        await commandBus.PublishAsync(command, default);

        // Return encryptedChatWaiting to the initiator
        return new TEncryptedChatWaiting
        {
            Id = chatId,
            AccessHash = accessHash,
            Date = date,
            AdminId = input.UserId,
            ParticipantId = toUserId
        };
    }
}
