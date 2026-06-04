// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// See <a href="https://corefork.telegram.org/method/bots.toggleUserEmojiStatusPermission" />
///</summary>
internal sealed class ToggleUserEmojiStatusPermissionHandler : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestToggleUserEmojiStatusPermission, IBool>,
    Bots.IToggleUserEmojiStatusPermissionHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestToggleUserEmojiStatusPermission obj)
    {
        throw new NotImplementedException();
    }
}
