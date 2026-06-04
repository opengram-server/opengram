namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Clear recent stickers
/// See <a href="https://corefork.telegram.org/method/messages.clearRecentStickers" />
///</summary>
internal sealed class ClearRecentStickersHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestClearRecentStickers, IBool>,
    Messages.IClearRecentStickersHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestClearRecentStickers obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
