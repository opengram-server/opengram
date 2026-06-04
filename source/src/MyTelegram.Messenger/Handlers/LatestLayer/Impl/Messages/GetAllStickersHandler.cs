namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Get all installed stickers
/// See <a href="https://corefork.telegram.org/method/messages.getAllStickers" />
///</summary>
internal sealed class GetAllStickersHandler(
    IQueryProcessor queryProcessor,
    IStickerSetConverter stickerSetConverter) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetAllStickers, MyTelegram.Schema.Messages.IAllStickers>,
    Messages.IGetAllStickersHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAllStickers> HandleCoreAsync(IRequestInput input,
        RequestGetAllStickers obj)
    {
        // Get all installed sticker sets for the user
        var installedStickerSets = await queryProcessor.ProcessAsync(
            new GetInstalledStickerSetsQuery(input.UserId, StickerSetType.Regular));

        if (installedStickerSets == null || !installedStickerSets.Any())
        {
            return new MyTelegram.Schema.Messages.TAllStickers { Sets = [] };
        }

        // Get the actual sticker set data
        var stickerSetIds = installedStickerSets.Select(x => x.StickerSetId).ToList();
        var stickerSets = await queryProcessor.ProcessAsync(
            new GetStickerSetsByIdListQuery(stickerSetIds));

        if (stickerSets == null || !stickerSets.Any())
        {
            return new MyTelegram.Schema.Messages.TAllStickers { Sets = [] };
        }

        // Convert to TL objects
        var sets = stickerSets
            .Select(s => stickerSetConverter.ToStickerSet(input.UserId, s))
            .ToList();

        var r = new MyTelegram.Schema.Messages.TAllStickers 
        { 
            Sets = new TVector<MyTelegram.Schema.IStickerSet>(sets),
            Hash = 0 // TODO: Calculate proper hash if needed
        };

        return r;
    }
}
