// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Sends a message with a file attachment to a secret chat
/// See <a href="https://corefork.telegram.org/method/messages.sendEncryptedFile" />
///</summary>
internal sealed class SendEncryptedFileHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSendEncryptedFile, MyTelegram.Schema.Messages.ISentEncryptedMessage>,
    Messages.ISendEncryptedFileHandler
{
    protected override async Task<MyTelegram.Schema.Messages.ISentEncryptedMessage> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSendEncryptedFile obj)
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
            Array.Empty<byte>(),
            SendMessageType.Media,
            date);

        await commandBus.PublishAsync(command, default);

        // Return TSentEncryptedFile with file reference
        return new MyTelegram.Schema.Messages.TSentEncryptedFile
        {
            Date = date,
            File = new TEncryptedFile
            {
                Id = Random.Shared.NextInt64(),
                AccessHash = Random.Shared.NextInt64(),
                Size = 0,
                DcId = 1,
                KeyFingerprint = 0
            }
        };
    }
}
