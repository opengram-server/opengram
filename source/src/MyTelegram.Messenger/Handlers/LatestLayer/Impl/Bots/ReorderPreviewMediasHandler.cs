// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Bots;

///<summary>
/// Reorder a <a href="https://corefork.telegram.org/api/bots/webapps#main-mini-app-previews">main mini app previews, see here »</a> for more info.Only owners of bots with a configured Main Mini App can use this method, see <a href="https://corefork.telegram.org/api/bots/webapps#main-mini-app-previews">see here »</a> for more info on how to check if you can invoke this method.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 BOT_INVALID This is not a valid bot.
/// See <a href="https://corefork.telegram.org/method/bots.reorderPreviewMedias" />
///</summary>
internal sealed class ReorderPreviewMediasHandler : RpcResultObjectHandler<MyTelegram.Schema.Bots.RequestReorderPreviewMedias, IBool>,
    Bots.IReorderPreviewMediasHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Bots.RequestReorderPreviewMedias obj)
    {
        throw new NotImplementedException();
    }
}
