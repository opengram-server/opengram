using MyTelegram.Queries;
using MyTelegram.Converters;
using MyTelegram.Services.CustomEmoji;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Gets the list of currently installed <a href="https://corefork.telegram.org/api/custom-emoji">custom emoji stickersets</a>.
/// See <a href="https://corefork.telegram.org/method/messages.getEmojiStickers" />
///</summary>
internal sealed class GetEmojiStickersHandler(
    IQueryProcessor queryProcessor,
    IStickerSetConverter stickerSetConverter,
    ILogger<GetEmojiStickersHandler> logger) 
    : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetEmojiStickers, MyTelegram.Schema.Messages.IAllStickers>,
    Messages.IGetEmojiStickersHandler
{
    protected override async Task<MyTelegram.Schema.Messages.IAllStickers> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetEmojiStickers obj)
    {
        logger.LogWarning(
            "*** GetEmojiStickers called by user {UserId} ***",
            input.UserId);

        // Get all installed custom emoji sticker sets for the user
        var installedStickerSets = await queryProcessor.ProcessAsync(
            new GetInstalledStickerSetsQuery(input.UserId, StickerSetType.CustomEmoji));

        logger.LogWarning(
            "*** Found {Count} installed custom emoji packs for user {UserId} ***",
            installedStickerSets?.Count ?? 0,
            input.UserId);

        if (installedStickerSets == null || !installedStickerSets.Any())
        {
            return new TAllStickers { Sets = [] };
        }

        // Get the actual sticker set data
        var stickerSetIds = installedStickerSets.Select(x => x.StickerSetId).ToList();
        var stickerSets = await queryProcessor.ProcessAsync(
            new GetStickerSetsByIdListQuery(stickerSetIds));

        if (stickerSets == null || !stickerSets.Any())
        {
            logger.LogWarning(
                "*** No sticker set data found for IDs: {Ids} ***",
                string.Join(", ", stickerSetIds));
            return new TAllStickers { Sets = [] };
        }

        // Get all documents for all sticker sets
        var allDocumentIds = stickerSets
            .Where(s => s.StickerDocumentIds != null)
            .SelectMany(s => s.StickerDocumentIds)
            .Distinct()
            .ToList();

        var allDocuments = await queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery(allDocumentIds));

        logger.LogWarning(
            "*** Loaded {DocCount} documents for emoji packs ***",
            allDocuments?.Count ?? 0);

        // Create sticker set with documents for each pack
        var sets = stickerSets.Select(s =>
        {
            var documentsForSet = allDocuments?
                .Where(doc => s.StickerDocumentIds?.Contains(doc.DocumentId) == true)
                .ToList() ?? new List<IDocumentReadModel>();

            // Convert IDocumentReadModel to IDocument
            var tlDocuments = documentsForSet.Select(doc => CustomEmojiConverter.ConvertDocumentToTl(doc, s, "🙂")).Cast<IDocument>().ToList();

            return stickerSetConverter.ToMessagesStickerSet(input.UserId, s, tlDocuments);
        }).ToList();

        logger.LogWarning(
            "*** Returning {Count} custom emoji packs to user {UserId} ***",
            sets.Count,
            input.UserId);

        // Calculate hash for caching (use sticker set count and max ID)
        var hash = sets.Count * 31 + (sets.Any() ? sets.First().GetHashCode() : 0);

        return new MyTelegram.Schema.Messages.TAllStickers 
        { 
            Sets = new TVector<MyTelegram.Schema.IStickerSet>(sets.Select(s => ((MyTelegram.Schema.Messages.TStickerSet)s).Set)),
            Hash = hash
        };
    }
}
