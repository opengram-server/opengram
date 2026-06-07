// ReSharper disable All

using MyTelegram.Domain.Aggregates.EncryptedChat;
using MyTelegram.Domain.Commands.EncryptedChat;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Cancels a request for creation and/or delete info on secret chat.
/// See <a href="https://corefork.telegram.org/method/messages.discardEncryption" />
///</summary>
internal sealed class DiscardEncryptionHandler(
    ICommandBus commandBus)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDiscardEncryption, IBool>,
    Messages.IDiscardEncryptionHandler
{
    protected override async Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDiscardEncryption obj)
    {
        if (obj.ChatId == 0)
        {
            RpcErrors.RpcErrors400.ChatIdEmpty.ThrowRpcError();
        }

        var command = new DiscardEncryptionCommand(
            EncryptedChatId.Create(obj.ChatId),
            input.ToRequestInfo(),
            obj.DeleteHistory);

        await commandBus.PublishAsync(command, default);

        return new TBoolTrue();
    }
}
