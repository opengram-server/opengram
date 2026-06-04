// ReSharper disable All

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Mark or unmark a sticker as favorite
/// <para>Possible errors</para>
/// Code Type Description
/// 400 STICKER_ID_INVALID The provided sticker ID is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.faveSticker" />
///</summary>
internal sealed class FaveStickerHandler(ILogger<FaveStickerHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestFaveSticker, IBool>,
    Messages.IFaveStickerHandler
{
    protected override Task<IBool> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestFaveSticker obj)
    {
        // Validate sticker ID
        if (obj.Id is not TInputDocument inputDocument)
        {
            RpcErrors.RpcErrors400.StickerIdInvalid.ThrowRpcError();
            throw new InvalidOperationException("Invalid sticker ID");
        }

        logger.LogInformation(
            "User {UserId} {Action} sticker {StickerId} {ActionType} favorites",
            input.UserId,
            obj.Unfave ? "removed" : "added",
            inputDocument.Id,
            obj.Unfave ? "from" : "to");

        // TODO: Full implementation:
        // - Save to FavoriteStickerReadModel in MongoDB
        // - Update messages.getFavedStickers response
        // For now, just return success to prevent crashes
        
        return Task.FromResult<IBool>(new TBoolTrue());
    }
}
