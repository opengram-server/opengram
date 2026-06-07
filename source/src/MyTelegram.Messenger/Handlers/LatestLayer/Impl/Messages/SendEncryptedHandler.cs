// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Sends a text message to a secret chat.
/// See <a href="https://corefork.telegram.org/method/messages.sendEncrypted" />
///</summary>
internal sealed class SendEncryptedHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendEncrypted, MyTelegram.Schema.Messages.ISentEncryptedMessage>,
    Messages.ISendEncryptedHandler
{
    protected override async Task<MyTelegram.Schema.Messages.ISentEncryptedMessage> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendEncrypted obj)
    {
        var chatId = obj.Peer.ChatId;
        var date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        if (obj.Data == null || obj.Data.Length == 0)
        {
            RpcErrors.RpcErrors400.DataInvalid.ThrowRpcError();
        }

        var command = new SendEncryptedMessageCommand(
            EncryptedChatId.Create(chatId),
            input.ToRequestInfo(),
            obj.RandomId,
            obj.Data,
            null,
            SendMessageType.Text,
            date);

        await commandBus.PublishAsync(command, default);

        return new MyTelegram.Schema.Messages.TSentEncryptedMessage
        {
            Date = date
        };
    }
}
