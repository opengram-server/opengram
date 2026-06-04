namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Apply changes to multiple stickersets
/// See <a href="https://corefork.telegram.org/method/messages.toggleStickerSets" />
///</summary>
internal sealed class ToggleStickerSetsHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestToggleStickerSets, IBool>,
    Messages.IToggleStickerSetsHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestToggleStickerSets obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
