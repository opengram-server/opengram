namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Confirms receipt of messages by a client, cancels PUSH-notification sending.
/// See <a href="https://corefork.telegram.org/method/messages.receivedMessages" />
///</summary>
internal sealed class ReceivedMessagesHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReceivedMessages, TVector<MyTelegram.Schema.IReceivedNotifyMessage>>,
    Messages.IReceivedMessagesHandler
{
    protected override Task<TVector<IReceivedNotifyMessage>> HandleCoreAsync(IRequestInput input,
        RequestReceivedMessages obj)
    {
        return Task.FromResult(new TVector<IReceivedNotifyMessage> { new TReceivedNotifyMessage { Id = obj.MaxId } });
    }
}
