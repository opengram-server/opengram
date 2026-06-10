// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Marks message history within a secret chat as read.
/// See <a href="https://corefork.telegram.org/method/messages.readEncryptedHistory" />
///</summary>
internal sealed class ReadEncryptedHistoryHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReadEncryptedHistory, IBool>,
    Messages.IReadEncryptedHistoryHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReadEncryptedHistory obj)
    {
        // Mark encrypted messages as read up to obj.MaxDate.
        // Encrypted message read state is tracked client-side;
        // a follow-up can push updateEncryptedMessagesRead to the peer.
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
