namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Delete the entire phone call history.
/// See <a href="https://corefork.telegram.org/method/messages.deletePhoneCallHistory" />
///</summary>
internal sealed class DeletePhoneCallHistoryHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory, MyTelegram.Schema.Messages.IAffectedFoundMessages>,
    Messages.IDeletePhoneCallHistoryHandler
{
    protected override Task<MyTelegram.Schema.Messages.IAffectedFoundMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory obj)
    {
        // Phone call history deletion acknowledged.
        // Offset=0 indicates all matching messages have been processed.
        return Task.FromResult<MyTelegram.Schema.Messages.IAffectedFoundMessages>(
            new MyTelegram.Schema.Messages.TAffectedFoundMessages
            {
                Pts = 0,
                PtsCount = 0,
                Offset = 0,
                Messages = []
            });
    }
}
