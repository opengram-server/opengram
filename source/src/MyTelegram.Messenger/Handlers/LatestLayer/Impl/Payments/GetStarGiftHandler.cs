using MyTelegram.Messenger.Handlers.LatestLayer.Interfaces.Payments;
using MyTelegram.Schema;
using MyTelegram.Queries.StarGift;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

public sealed class GetStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarGift, MyTelegram.Schema.IStarGift>,
    IGetStarGiftHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetStarGiftHandler> _logger;

    public GetStarGiftHandler(IQueryProcessor queryProcessor, ILogger<GetStarGiftHandler> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.IStarGift> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarGift obj)
    {
        _logger.LogInformation("GetStarGift - UserId={UserId}, MsgId={MsgId}", input.UserId, obj.MsgId);

        // MsgId=0 — особый случай от старого клиента. Возвращаем первый доступный подарок.
        if (obj.MsgId == 0)
        {
            _logger.LogWarning("GetStarGift called with MsgId=0 by user {UserId}. Attempting to return first available gift.", input.UserId);

            // Сначала пробуем взять подарок-заглушку с GiftId=0
            var placeholderGift = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftByIdQuery(0));

            // Если его нет, берём первый доступный подарок из каталога
            if (placeholderGift == null)
            {
                _logger.LogInformation("Placeholder gift with GiftId=0 not found, getting first available gift from catalog");
                var allGifts = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftsQuery(0)); // Hash=0 означает без кеширования
                
                if (allGifts == null || allGifts.Count == 0)
                {
                    _logger.LogError("No gifts found in catalog!");
                    throw new RpcException(new RpcError(400, "GIFT_ID_INVALID"));
                }
                
                placeholderGift = allGifts.First();
            }
            
            var firstGift = placeholderGift;
            _logger.LogInformation("Returning placeholder gift {GiftId} for MsgId=0", firstGift.GiftId);

            // Загружаем стикер
            var sticker = await LoadStickerDocumentAsync(firstGift.Sticker);
            if (sticker == null)
            {
                _logger.LogError("Failed to load sticker for fallback gift {GiftId}", firstGift.GiftId);
                throw new RpcException(new RpcError(500, "STICKER_UNAVAILABLE"));
            }
            
            // Проверяем ConvertStars
            if (firstGift.ConvertStars <= 0)
            {
                _logger.LogError("Fallback gift {GiftId} has invalid ConvertStars", firstGift.GiftId);
                throw new RpcException(new RpcError(500, "INVALID_GIFT_DATA"));
            }

            // Собираем подарок из каталога
            var gift = new TStarGift
            {
                Id = firstGift.GiftId,
                Sticker = sticker,
                Stars = firstGift.Stars,
                ConvertStars = firstGift.ConvertStars,
                
                Limited = firstGift.Limited,
                SoldOut = firstGift.SoldOut,
                Birthday = firstGift.Birthday,
                // Убрано ради совместимости с клиентом 5.16.0
                // RequirePremium = firstGift.RequirePremium,
                // LimitedPerUser = firstGift.LimitedPerUser,

                AvailabilityRemains = firstGift.Limited ? firstGift.AvailabilityRemains : null,
                AvailabilityTotal = firstGift.Limited ? firstGift.AvailabilityTotal : null,
                AvailabilityResale = firstGift.AvailabilityResale,

                FirstSaleDate = firstGift.FirstSaleDate,
                LastSaleDate = firstGift.LastSaleDate,
                // Убрано ради совместимости с клиентом 5.16.0
                // LockedUntilDate = firstGift.LockedUntilDate,

                UpgradeStars = firstGift.UpgradeStars,
                ResellMinStars = firstGift.ResellMinStars,

                Title = string.IsNullOrWhiteSpace(firstGift.Title) ? null : firstGift.Title,

                // Убрано ради совместимости с клиентом 5.16.0
                // PerUserTotal = firstGift.LimitedPerUser ? firstGift.PerUserTotal : null,
                // PerUserRemains = firstGift.LimitedPerUser ? firstGift.PerUserRemains : null,
            };

            _logger.LogInformation("Returning fallback gift {GiftId} for MsgId=0", gift.Id);
            return gift;
        }
        
        // Обычный случай: получаем подарок из полученных сообщений
        var giftReadModel = await _queryProcessor.ProcessAsync(new GetStarGiftByMessageIdQuery(input.UserId, obj.MsgId));
        
        if (giftReadModel == null)
        {
            _logger.LogWarning("Gift not found for user {UserId}, msgId {MsgId}", input.UserId, obj.MsgId);
            throw new RpcException(new RpcError(400, "GIFT_ID_INVALID"));
        }

        return await BuildStarGiftFromReadModelAsync(giftReadModel);
    }
    
    private async Task<IStarGift> BuildStarGiftFromReadModelAsync(MyTelegram.ReadModel.Interfaces.IStarGiftReadModel readModel)
    {
        // Загружаем документ стикера
        var sticker = await LoadStickerDocumentAsync(readModel.StickerDocumentId);

        if (sticker == null)
        {
            _logger.LogError("Failed to load sticker for gift {GiftId}", readModel.GiftId);
            throw new RpcException(new RpcError(500, "STICKER_UNAVAILABLE"));
        }

        // Проверяем ConvertStars
        if (readModel.ConvertStars <= 0)
        {
            _logger.LogError("Gift {GiftId} has invalid ConvertStars ({ConvertStars})", readModel.GiftId, readModel.ConvertStars);
            throw new RpcException(new RpcError(500, "INVALID_GIFT_DATA"));
        }

        // Берём метаданные подарка из каталога
        var catalogGift = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftByIdQuery(readModel.GiftId));
        
        var gift = new TStarGift
        {
            Id = readModel.GiftId,
            Sticker = sticker,
            Stars = readModel.Stars,
            ConvertStars = readModel.ConvertStars,
            
            // Метаданные из каталога (если они есть)
            Limited = catalogGift?.Limited ?? false,
            SoldOut = catalogGift?.SoldOut ?? false,
            Birthday = catalogGift?.Birthday ?? false,
            // Убрано ради совместимости с клиентом 5.16.0
            // RequirePremium = catalogGift?.RequirePremium ?? false,
            // LimitedPerUser = catalogGift?.LimitedPerUser ?? false,

            AvailabilityRemains = catalogGift?.Limited == true ? catalogGift.AvailabilityRemains : null,
            AvailabilityTotal = catalogGift?.Limited == true ? catalogGift.AvailabilityTotal : null,
            AvailabilityResale = catalogGift?.AvailabilityResale,

            FirstSaleDate = catalogGift?.FirstSaleDate,
            LastSaleDate = catalogGift?.LastSaleDate,
            // Убрано ради совместимости с клиентом 5.16.0
            // LockedUntilDate = catalogGift?.LockedUntilDate,

            UpgradeStars = readModel.UpgradeStars ?? catalogGift?.UpgradeStars,
            ResellMinStars = catalogGift?.ResellMinStars,

            // Берём Title из каталога, так как в readModel он не хранится
            Title = !string.IsNullOrWhiteSpace(catalogGift?.Title) ? catalogGift.Title :
                    (!string.IsNullOrWhiteSpace(readModel.Title) ? readModel.Title : null),

            // Убрано ради совместимости с клиентом 5.16.0
            // PerUserTotal = catalogGift?.LimitedPerUser == true ? catalogGift.PerUserTotal : null,
            // PerUserRemains = catalogGift?.LimitedPerUser == true ? catalogGift.PerUserRemains : null,
        };

        _logger.LogInformation("Returning gift {GiftId} to user", readModel.GiftId);
        return gift;
    }
    
    private async Task<TDocument?> LoadStickerDocumentAsync(long? stickerId)
    {
        if (!stickerId.HasValue || stickerId.Value <= 0)
        {
            return null;
        }
        
        var document = await _queryProcessor.ProcessAsync(new GetDocumentByIdQuery(stickerId.Value));
        
        if (document == null)
        {
            _logger.LogWarning("Document {DocId} not found", stickerId.Value);
            return null;
        }
        
        if (document.FileReference.IsEmpty)
        {
            _logger.LogWarning("Document {DocId} has no FileReference", document.DocumentId);
            return null;
        }
        
        if (string.IsNullOrEmpty(document.MimeType))
        {
            _logger.LogWarning("Document {DocId} has no MimeType", document.DocumentId);
            return null;
        }
        
        var sticker = new TDocument
        {
            Id = document.DocumentId,
            AccessHash = document.AccessHash,
            FileReference = document.FileReference.ToArray(),
            Date = document.Date,
            MimeType = document.MimeType,
            Size = document.Size,
            DcId = document.DcId,
            Attributes = new TVector<IDocumentAttribute>(document.Attributes2 ?? new List<IDocumentAttribute>())
        };
        
        // Переносим миниатюры
        if (document.Thumbs != null)
        {
            sticker.Thumbs = new TVector<IPhotoSize>(document.Thumbs.Select(p => p.Type switch
            {
                "i" => (IPhotoSize)new TPhotoStrippedSize { Type = p.Type, Bytes = p.Bytes },
                "j" => new TPhotoPathSize { Type = p.Type, Bytes = p.Bytes },
                _ => new TPhotoSize
                {
                    Type = p.Type,
                    W = p.W,
                    H = p.H,
                    Size = (int)p.Size
                }
            }));
        }
        
        // Переносим видео-миниатюры
        if (document.VideoThumbs != null)
        {
            sticker.VideoThumbs = new TVector<IVideoSize>(document.VideoThumbs.Select(p => new TVideoSize
            {
                Type = p.Type,
                W = p.W,
                H = p.H,
                Size = (int)p.Size,
                VideoStartTs = p.VideoStartTs
            }));
        }
        
        return sticker;
    }
}
