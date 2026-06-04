namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Возвращает популярные (featured) наборы кастомных эмодзи.
/// See <a href="https://corefork.telegram.org/method/messages.getFeaturedEmojiStickers" />
///</summary>
internal sealed class GetFeaturedEmojiStickersHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetFeaturedEmojiStickers, MyTelegram.Schema.Messages.IFeaturedStickers>,
    Messages.IGetFeaturedEmojiStickersHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetFeaturedEmojiStickersHandler> _logger;

    public GetFeaturedEmojiStickersHandler(
        IQueryProcessor queryProcessor,
        ILogger<GetFeaturedEmojiStickersHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Messages.IFeaturedStickers> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetFeaturedEmojiStickers obj)
    {
        _logger.LogInformation("GetFeaturedEmojiStickers called - UserId={UserId}, Hash={Hash}", input.UserId, obj.Hash);

        // Загружаем популярные наборы кастомных эмодзи
        var featuredSets = await _queryProcessor.ProcessAsync(
            new GetFeaturedStickerSetsQuery(StickerSetType.CustomEmoji),
            CancellationToken.None);

        if (featuredSets.Count == 0)
        {
            _logger.LogWarning("No featured emoji sticker sets found");
            return new TFeaturedStickers
            {
                Hash = 0,
                Count = 0,
                Sets = new TVector<IStickerSetCovered>(),
                Unread = new TVector<long>()
            };
        }

        var sets = new List<IStickerSetCovered>();
        
        foreach (var stickerSet in featuredSets)
        {
            // Берём документ обложки набора
            var coverDocumentId = stickerSet.Covers.FirstOrDefault();
            if (coverDocumentId == 0)
            {
                coverDocumentId = stickerSet.StickerDocumentIds.FirstOrDefault();
            }

            if (coverDocumentId > 0)
            {
                var coverDoc = await _queryProcessor.ProcessAsync(
                    new GetDocumentByIdQuery(coverDocumentId),
                    CancellationToken.None);

                if (coverDoc != null)
                {
                    sets.Add(new TStickerSetCovered
                    {
                        Set = new MyTelegram.Schema.TStickerSet
                        {
                            Id = stickerSet.StickerSetId,
                            AccessHash = stickerSet.AccessHash,
                            Title = stickerSet.Title,
                            ShortName = stickerSet.ShortName,
                            Count = stickerSet.Count,
                            Hash = 0,
                            Masks = stickerSet.Masks,
                            Emojis = stickerSet.Emojis,
                            TextColor = stickerSet.TextColor,
                            ChannelEmojiStatus = stickerSet.ChannelEmojiStatus
                        },
                        Cover = new TDocument
                        {
                            Id = coverDoc.DocumentId,
                            AccessHash = coverDoc.AccessHash,
                            FileReference = coverDoc.FileReference.IsEmpty ? Array.Empty<byte>() : coverDoc.FileReference.ToArray(),
                            Date = coverDoc.Date,
                            MimeType = coverDoc.MimeType ?? "application/x-tgsticker",
                            Size = coverDoc.Size,
                            DcId = coverDoc.DcId,
                            Attributes = coverDoc.Attributes2 != null
                                ? new TVector<IDocumentAttribute>(coverDoc.Attributes2)
                                : new TVector<IDocumentAttribute>()
                        }
                    });
                }
            }
        }

        _logger.LogInformation("Returning {Count} featured emoji sticker sets", sets.Count);

        return new TFeaturedStickers
        {
            Hash = 0,  // TODO: рассчитать корректный hash
            Count = sets.Count,
            Sets = new TVector<IStickerSetCovered>(sets),
            Unread = new TVector<long>()  // TODO: учитывать непрочитанные наборы
        };
    }
}
