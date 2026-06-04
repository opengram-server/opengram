namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// See <a href="https://corefork.telegram.org/method/messages.savePreparedInlineMessage" />
///</summary>
internal sealed class SavePreparedInlineMessageHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSavePreparedInlineMessage, MyTelegram.Schema.Messages.IBotPreparedInlineMessage>,
    Messages.ISavePreparedInlineMessageHandler
{
    protected override Task<MyTelegram.Schema.Messages.IBotPreparedInlineMessage> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestSavePreparedInlineMessage obj)
    {
        throw new NotImplementedException();
    }
}
