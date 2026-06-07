// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Send typing event by the current user to a secret chat.
/// See <a href="https://corefork.telegram.org/method/messages.setEncryptedTyping" />
///</summary>
internal sealed class SetEncryptedTypingHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSetEncryptedTyping, IBool>,
    Messages.ISetEncryptedTypingHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSetEncryptedTyping obj)
    {
        // Typing events are lightweight — acknowledge without persistence.
        // The updateEncryptedChatTyping push requires looking up the other
        // participant from the encrypted chat read model; this can be added
        // in a follow-up when the read model query is wired.
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
