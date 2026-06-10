namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Add/remove sticker from recent stickers list.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 STICKER_ID_INVALID The provided sticker ID is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.saveRecentSticker" />
///</summary>
internal sealed class SaveRecentStickerHandler
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestSaveRecentSticker, IBool>,
        Messages.ISaveRecentStickerHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        RequestSaveRecentSticker obj)
    {
        // Recent stickers are typically managed client-side.
        // Server acknowledges the request.
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
