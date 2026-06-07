// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Sends a service message to a secret chat.
/// See <a href="https://corefork.telegram.org/method/messages.sendEncryptedService" />
///</summary>
internal sealed class SendEncryptedServiceHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendEncryptedService, MyTelegram.Schema.Messages.ISentEncryptedMessage>,
    Messages.ISendEncryptedServiceHandler
{
    protected override async Task<MyTelegram.Schema.Messages.ISentEncryptedMessage> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendEncryptedService obj)
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
            SendMessageType.MessageService,
            date);

        await commandBus.PublishAsync(command, default);

        return new MyTelegram.Schema.Messages.TSentEncryptedMessage
        {
            Date = date
        };
    }
}
