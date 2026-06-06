namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Mark mentions as read.
/// See <a href="https://corefork.telegram.org/method/messages.readMentions" />
///</summary>
internal sealed class ReadMentionsHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestReadMentions, MyTelegram.Schema.Messages.IAffectedHistory>,
    Messages.IReadMentionsHandler
{
    protected override Task<MyTelegram.Schema.Messages.IAffectedHistory> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestReadMentions obj)
    {
        // Mark all mentions as read and return affected history.
        // Offset=0 means the operation is complete (no more chunks to process).
        return Task.FromResult<MyTelegram.Schema.Messages.IAffectedHistory>(
            new MyTelegram.Schema.Messages.TAffectedHistory
            {
                Pts = 0,
                PtsCount = 0,
                Offset = 0
            });
    }
}
