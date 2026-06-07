namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Delete the entire phone call history.
/// See <a href="https://corefork.telegram.org/method/messages.deletePhoneCallHistory" />
///</summary>
internal sealed class DeletePhoneCallHistoryHandler(
    IPtsHelper ptsHelper)
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory, MyTelegram.Schema.Messages.IAffectedFoundMessages>,
    Messages.IDeletePhoneCallHistoryHandler
{
    protected override Task<MyTelegram.Schema.Messages.IAffectedFoundMessages> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestDeletePhoneCallHistory obj)
    {
        // Phone calls are not currently tracked in the messaging layer.
        // Return current PTS with zero affected messages to indicate success.
        var pts = ptsHelper.GetCachedPts(input.UserId);

        return Task.FromResult<MyTelegram.Schema.Messages.IAffectedFoundMessages>(
            new MyTelegram.Schema.Messages.TAffectedFoundMessages
            {
                Pts = pts,
                PtsCount = 0,
                Offset = 0,
                Messages = []
            });
    }
}
