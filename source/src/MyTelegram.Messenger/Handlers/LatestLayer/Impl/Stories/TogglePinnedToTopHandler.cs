// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.togglePinnedToTop" />
///</summary>
internal sealed class TogglePinnedToTopHandler : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestTogglePinnedToTop, IBool>,
    Stories.ITogglePinnedToTopHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestTogglePinnedToTop obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
