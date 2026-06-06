namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get message ranges for saving the user's chat history on a new device.
/// See <a href="https://corefork.telegram.org/method/messages.getSplitRanges" />
///</summary>
internal sealed class GetSplitRangesHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetSplitRanges, TVector<MyTelegram.Schema.IMessageRange>>,
    Messages.IGetSplitRangesHandler
{
    protected override Task<TVector<IMessageRange>> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetSplitRanges obj)
    {
        // Return a single range covering the entire message history.
        // On a self-hosted server without DC migration, there is no split.
        var range = new TMessageRange
        {
            MinId = 1,
            MaxId = int.MaxValue
        };

        return Task.FromResult(new TVector<IMessageRange>(new List<IMessageRange> { range }));
    }
}
