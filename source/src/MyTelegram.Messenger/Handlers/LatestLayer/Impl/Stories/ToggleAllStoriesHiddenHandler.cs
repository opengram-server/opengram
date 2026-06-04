// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Stories;

///<summary>
/// See <a href="https://corefork.telegram.org/method/stories.toggleAllStoriesHidden" />
///</summary>
internal sealed class ToggleAllStoriesHiddenHandler : RpcResultObjectHandler<MyTelegram.Schema.Stories.RequestToggleAllStoriesHidden, IBool>,
    Stories.IToggleAllStoriesHiddenHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Stories.RequestToggleAllStoriesHidden obj)
    {
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
