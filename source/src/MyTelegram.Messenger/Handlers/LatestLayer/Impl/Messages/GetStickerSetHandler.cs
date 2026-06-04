using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.Converters;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Messages;

///<summary>
/// Возвращает информацию о наборе стикеров.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 EMOTICON_STICKERPACK_MISSING inputStickerSetDice.emoji cannot be empty.
/// 406 STICKERSET_INVALID The provided sticker set is invalid.
/// See <a href="https://corefork.telegram.org/method/messages.getStickerSet" />
///</summary>
internal sealed class GetStickerSetHandler : RpcResultObjectHandler<MyTelegram.Schema.Messages.RequestGetStickerSet, MyTelegram.Schema.Messages.IStickerSet>,
    Messages.IGetStickerSetHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetStickerSetHandler> _logger;

    public GetStickerSetHandler(
        IQueryProcessor queryProcessor,
        ILogger<GetStickerSetHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Messages.IStickerSet> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Messages.RequestGetStickerSet obj)
    {
        _logger.LogInformation(
            "User {UserId} requesting sticker set: {StickerSet}",
            input.UserId,
            obj.Stickerset);

        // Получаем stickerset_id из InputStickerSet
        long stickerSetId = 0;
        string? shortName = null;

        switch (obj.Stickerset)
        {
            case TInputStickerSetID inputId:
                // Перехватываем запрос самодельного набора Star Gifts по ID
                if (inputId.Id == 7770001)
                {
                    return await GenerateCustomStarGiftsSetAsync();
                }

                stickerSetId = inputId.Id;
                break;
            case TInputStickerSetShortName inputShortName:
                // Перехватываем запрос самодельного набора Star Gifts по короткому имени.
                // Это позволяет открыть набор по ссылке: t.me/addstickers/custom_star_gifts
                if (inputShortName.ShortName == "custom_star_gifts" || inputShortName.ShortName == "star_gifts")
                {
                    return await GenerateCustomStarGiftsSetAsync();
                }

                shortName = inputShortName.ShortName;
                break;
            case TInputStickerSetAnimatedEmoji:
            case TInputStickerSetAnimatedEmojiAnimations:
            case TInputStickerSetEmojiGenericAnimations:
            case TInputStickerSetEmojiDefaultStatuses:
            case TInputStickerSetEmojiDefaultTopicIcons:
            case TInputStickerSetEmojiChannelDefaultStatuses:
                // Это специальные системные наборы для анимированных эмодзи.
                // Возвращаем пустой набор вместо NotModified, чтобы не было ошибок разбора.
                _logger.LogInformation("Returning empty sticker set for system type: {Type}", obj.Stickerset.GetType().Name);
                return new MyTelegram.Schema.Messages.TStickerSet
                {
                    Set = new MyTelegram.Schema.TStickerSet
                    {
                        Id = 0,
                        AccessHash = 0,
                        Title = "System Emoji",
                        ShortName = "",
                        Count = 0,
                        Hash = 0
                    },
                    Packs = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerPack>(),
                    Keywords = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerKeyword>(),
                    Documents = new MyTelegram.Schema.TVector<MyTelegram.Schema.IDocument>()
                };
            case TInputStickerSetPremiumGifts:
                // Возвращаем пустой набор для Premium Gifts (не реализовано).
                // Premium Gifts отличаются от Star Gifts: они показываются при дарении подписки Telegram Premium.
                _logger.LogInformation("TInputStickerSetPremiumGifts requested - returning empty set (Premium Gifts not implemented)");
                return new MyTelegram.Schema.Messages.TStickerSet
                {
                    Set = new MyTelegram.Schema.TStickerSet
                    {
                        Id = 0,
                        AccessHash = 0,
                        Title = "Premium Gifts",
                        ShortName = "premium_gifts",
                        Count = 0,
                        Hash = 0
                    },
                    Packs = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerPack>(),
                    Keywords = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerKeyword>(),
                    Documents = new MyTelegram.Schema.TVector<MyTelegram.Schema.IDocument>()
                };
            case TInputStickerSetTonGifts:
                // Возвращаем набор Star Gifts.
                // Star Gifts покупаются за Telegram Stars (работают на базе блокчейна TON).
                _logger.LogInformation("TInputStickerSetTonGifts requested - returning Star Gifts sticker set");
                return await GenerateCustomStarGiftsSetAsync();
            default:
                _logger.LogWarning("Unsupported InputStickerSet type: {Type}", obj.Stickerset.GetType().Name);
                RpcErrors.RpcErrors406.StickersetInvalid.ThrowRpcError();
                break;
        }

        // Запрашиваем из БД
        var stickerSetReadModel = shortName != null
            ? await _queryProcessor.ProcessAsync(new GetStickerSetByNameQuery(shortName), CancellationToken.None)
            : await _queryProcessor.ProcessAsync(new GetStickerSetByIdQuery(stickerSetId), CancellationToken.None);

        if (stickerSetReadModel == null)
        {
            // Запасной путь для star_gifts, если набора нет в базе - генерируем динамически
            if (shortName == "star_gifts")
            {
                 return await GenerateCustomStarGiftsSetAsync();
            }

            _logger.LogWarning("Sticker set not found: id={Id}, shortName={ShortName}", stickerSetId, shortName);
            RpcErrors.RpcErrors406.StickersetInvalid.ThrowRpcError();
        }

        // Получаем документы стикеров
        var documents = await _queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery((IList<long>)stickerSetReadModel!.StickerDocumentIds),
            CancellationToken.None);

        _logger.LogInformation(
            "Found {Count} documents for sticker set {Id}, requested {RequestedCount} IDs",
            documents.Count,
            stickerSetReadModel.StickerSetId,
            stickerSetReadModel.StickerDocumentIds.Count);
        
        if (documents.Count == 0)
        {
            _logger.LogWarning(
                "No documents found! StickerDocumentIds: [{Ids}]",
                string.Join(", ", stickerSetReadModel.StickerDocumentIds));
        }
        else
        {
            foreach (var doc in documents.Take(3))
            {
                _logger.LogInformation(
                    "Document: Id={DocId}, MimeType={MimeType}, Size={Size}, FileRef={FileRefLength}, DcId={DcId}",
                    doc.DocumentId,
                    doc.MimeType,
                    doc.Size,
                    doc.FileReference.IsEmpty ? 0 : doc.FileReference.Length,
                    doc.DcId);
            }
        }

        // Создаем map документ ID -> эмодзи из Packs
        var docIdToEmoji = new Dictionary<long, string>();
        foreach (var pack in stickerSetReadModel.Packs)
        {
            foreach (var docId in pack.Documents)
            {
                docIdToEmoji[docId] = pack.Emoticon;
            }
        }

        // Конвертируем документы в TL Document
        var tlDocuments = documents.Select(doc =>
        {
            var emoji = docIdToEmoji.GetValueOrDefault(doc.DocumentId, "🙂");
            return CustomEmojiConverter.ConvertDocumentToTl(doc, stickerSetReadModel, emoji);
        }).Cast<MyTelegram.Schema.IDocument>().ToList();

        // Конвертируем Packs
        var tlPacks = stickerSetReadModel.Packs
            .Select(p => new MyTelegram.Schema.TStickerPack
            {
                Emoticon = p.Emoticon,
                Documents = new MyTelegram.Schema.TVector<long>(p.Documents)
            })
            .Cast<MyTelegram.Schema.IStickerPack>()
            .ToList();

        // Создаем TL StickerSet
        var tlSet = new MyTelegram.Schema.TStickerSet
        {
            Id = stickerSetReadModel.StickerSetId,
            AccessHash = stickerSetReadModel.AccessHash,
            Title = stickerSetReadModel.Title,
            ShortName = stickerSetReadModel.ShortName,
            Count = stickerSetReadModel.Count,
            Emojis = stickerSetReadModel.Emojis,
            TextColor = stickerSetReadModel.TextColor,
            ChannelEmojiStatus = stickerSetReadModel.ChannelEmojiStatus,
            Masks = stickerSetReadModel.Masks,
            Archived = false,
            Official = false,
            Animated = documents.Any(p => p.MimeType == "application/x-tgsticker"),
            Videos = documents.Any(p => p.MimeType == "video/webm")
        };

        // Thumbnails
        if (stickerSetReadModel.Thumbs != null && stickerSetReadModel.Thumbs.Count > 0)
        {
            tlSet.Thumbs = new MyTelegram.Schema.TVector<MyTelegram.Schema.IPhotoSize>(
                stickerSetReadModel.Thumbs.Select(t => new MyTelegram.Schema.TPhotoSize
                {
                    Type = t.Type,
                    W = t.W,
                    H = t.H,
                    Size = (int)t.Size
                }).Cast<MyTelegram.Schema.IPhotoSize>().ToList());
        }

        if (stickerSetReadModel.ThumbDocumentId.HasValue)
        {
            tlSet.ThumbDocumentId = stickerSetReadModel.ThumbDocumentId.Value;
        }

        var result = new MyTelegram.Schema.Messages.TStickerSet
        {
            Set = tlSet,
            Packs = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerPack>(tlPacks),
            Keywords = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerKeyword>(), // пока пусто
            Documents = new MyTelegram.Schema.TVector<MyTelegram.Schema.IDocument>(tlDocuments)
        };

        _logger.LogInformation(
            "Returning sticker set {Id} with {DocCount} documents, {PackCount} packs. Set flags: Emojis={Emojis}, Masks={Masks}, TextColor={TextColor}",
            stickerSetReadModel.StickerSetId,
            tlDocuments.Count,
            tlPacks.Count,
            tlSet.Emojis,
            tlSet.Masks,
            tlSet.TextColor);
        
        if (tlDocuments.Count > 0)
        {
            var firstDoc = tlDocuments[0] as TDocument;
            _logger.LogInformation(
                "First document: Id={DocId}, Attributes count={AttrCount}, Thumbs count={ThumbsCount}",
                firstDoc?.Id,
                firstDoc?.Attributes?.Count ?? 0,
                firstDoc?.Thumbs?.Count ?? 0);
        }

        return result;
    }

    private async Task<MyTelegram.Schema.Messages.TStickerSet> GenerateCustomStarGiftsSetAsync()
    {
        _logger.LogInformation("Generating custom Star Gifts sticker set (Id=7770001, ShortName=star_gifts)");

        // 1. Загружаем все доступные подарки, чтобы получить их стикеры.
        // Передаём hash=0, чтобы получить все подарки.
        var allGifts = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftsQuery(0));

        // 2. Собираем уникальные идентификаторы документов
        var docIds = allGifts
            .Where(g => g.Sticker.HasValue)
            .Select(g => g.Sticker.Value)
            .Distinct()
            .ToList();

        // 3. Загружаем документы
        var giftDocuments = await _queryProcessor.ProcessAsync(
            new GetDocumentsByIdsQuery(docIds),
            CancellationToken.None);

        // 4. Конвертируем в TL Documents
        var tlGiftDocuments = giftDocuments.Select(doc =>
        {
            // Используем эмодзи по умолчанию для подарков
            var tlDoc = CustomEmojiConverter.ConvertDocumentToTl(doc, null, "🎁");

            // Отладочный лог
            if (tlDoc is TDocument tDoc)
            {
                _logger.LogInformation("   Converting Document {Id}: Size={Size}, Attributes={Count}",
                    tDoc.Id, tDoc.Size, tDoc.Attributes?.Count ?? 0);
                
                var stickerAttr = tDoc.Attributes?.FirstOrDefault(a => a is TDocumentAttributeSticker) as TDocumentAttributeSticker;
                if (stickerAttr != null)
                {
                    _logger.LogInformation("      StickerAttr: Stickerset={Type}",
                        stickerAttr.Stickerset?.GetType().Name);
                }
            }
            
            return tlDoc;
        }).Cast<MyTelegram.Schema.IDocument>().ToList();
        
        _logger.LogInformation("Converted {Count} documents for star_gifts sticker set", tlGiftDocuments.Count);

        // 5. Создаём Packs (все стикеры привязаны к эмодзи подарка)
        var pack = new MyTelegram.Schema.TStickerPack
        {
            Emoticon = "🎁",
            Documents = new MyTelegram.Schema.TVector<long>(docIds)
        };
        
        return new MyTelegram.Schema.Messages.TStickerSet
        {
            Set = new MyTelegram.Schema.TStickerSet
            {
                Id = 7770001,
                AccessHash = 7770001,
                Title = "Star Gifts",
                ShortName = "star_gifts",
                Count = tlGiftDocuments.Count,
                Hash = 0,
                Official = true, // помечаем как официальный для лучшего вида
                Animated = true, // TGS-стикеры
                Emojis = false
            },
            Packs = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerPack> { pack },
            Keywords = new MyTelegram.Schema.TVector<MyTelegram.Schema.IStickerKeyword>(),
            Documents = new MyTelegram.Schema.TVector<MyTelegram.Schema.IDocument>(tlGiftDocuments)
        };
    }
}
