// ReSharper disable All

using System.Security.Cryptography;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Schema.Payments;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// Get a list of available <a href="https://corefork.telegram.org/api/gifts">gifts, see here »</a> for more info.
/// See <a href="https://corefork.telegram.org/method/payments.getStarGifts" />
///</summary>
internal sealed class GetStarGiftsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetStarGifts, MyTelegram.Schema.Payments.IStarGifts>,
    Payments.IGetStarGiftsHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetStarGiftsHandler> _logger;
    private readonly IMongoDatabase _mongoDatabase;

    public GetStarGiftsHandler(IQueryProcessor queryProcessor, ILogger<GetStarGiftsHandler> logger, IMongoDatabase mongoDatabase)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
        _mongoDatabase = mongoDatabase;
    }

    protected override async Task<MyTelegram.Schema.Payments.IStarGifts> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetStarGifts obj)
    {
        _logger.LogInformation("GetStarGifts called - UserId={UserId}, Hash={Hash}", input.UserId, obj.Hash);
        var availableGifts = await _queryProcessor.ProcessAsync(new GetAvailableStarGiftsQuery(obj.Hash));
        _logger.LogInformation("Found {Count} available gifts in DB", availableGifts?.Count ?? 0);

        // Загружаем количество подарков, выставленных на перепродажу, чтобы показать их в каталоге
        var resaleCounts = new Dictionary<long, long>();
        try
        {
            var giftsCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
            var resaleFilter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("ForResale", true),
                Builders<BsonDocument>.Filter.Eq("Upgraded", true)
            );
            var resaleGiftsCursor = await giftsCollection.FindAsync(resaleFilter);
            var resaleGifts = await resaleGiftsCursor.ToListAsync();
            
            foreach (var gift in resaleGifts)
            {
                var giftId = gift.GetValue("GiftId", 0L).ToInt64();
                if (!resaleCounts.ContainsKey(giftId))
                {
                    resaleCounts[giftId] = 0;
                }
                resaleCounts[giftId]++;
            }
            
            _logger.LogInformation("Found resale counts for {Count} gift types: {Counts}",
                resaleCounts.Count, string.Join(", ", resaleCounts.Select(kv => $"{kv.Key}={kv.Value}")));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load resale counts");
        }

        if (availableGifts == null || !availableGifts.Any())
        {
            _logger.LogWarning("No gifts found in database! Run: node stargift-admin/seed-test-gifts-direct.js");
            return new TStarGifts
            {
                Hash = 0,
                Gifts = new TVector<IStarGift>(),
                Chats = new TVector<IChat>(),
                Users = new TVector<IUser>() 
            };
        }
        
        // Считаем текущий хеш по данным подарков
        var currentHash = ComputeGiftsHash(availableGifts.ToList());

        // Проверяем, есть ли у клиента актуальная версия (сверка по хешу)
        if (obj.Hash != 0 && obj.Hash == currentHash)
        {
            _logger.LogInformation("Hash matched ({Hash}), returning TStarGiftsNotModified", currentHash);
            return new TStarGiftsNotModified();
        }
        
        var giftsList = new List<IStarGift>();
        
        foreach (var g in availableGifts)
        {
            _logger.LogInformation("Processing gift {GiftId} with StickerId {StickerId}", g.GiftId, g.Sticker);

            TDocument? sticker = null;
            bool skipGift = false;
            
            if (g.Sticker.HasValue && g.Sticker.Value > 0)
            {
                try
                {
                    _logger.LogInformation("Loading document {DocId} for gift {GiftId}", g.Sticker.Value, g.GiftId);
                    var document = await _queryProcessor.ProcessAsync(new GetDocumentByIdQuery(g.Sticker.Value));
                    
                    if (document != null)
                    {
                        _logger.LogInformation("Document {DocId} loaded. FileReference length: {FileRefLength}, MimeType: {MimeType}, Attributes2: {AttrCount}",
                            document.DocumentId, 
                            document.FileReference.Length, 
                            document.MimeType ?? "null",
                            document.Attributes2?.Count() ?? 0);

                        // Проверяем, что у документа есть file reference
                        if (document.FileReference.IsEmpty)
                        {
                            _logger.LogWarning("Document {DocId} for gift {GiftId} has no file reference", document.DocumentId, g.GiftId);
                            skipGift = true;
                        }
                        // Проверяем MimeType
                        else if (string.IsNullOrEmpty(document.MimeType))
                        {
                            _logger.LogWarning("Document {DocId} for gift {GiftId} has no MimeType", document.DocumentId, g.GiftId);
                            skipGift = true;
                        }
                        else
                        {
                            // Собираем атрибуты документа
                            var attributes = new TVector<IDocumentAttribute>();
                            if (document.Attributes2 != null)
                            {
                                _logger.LogInformation("Document {DocId} has {Count} attributes", document.DocumentId, document.Attributes2.Count);
                                foreach (var attr in document.Attributes2)
                                {
                                    _logger.LogInformation("   Attribute: {Type}", attr.GetType().Name);
                                    if (attr is TDocumentAttributeSticker stickerAttr)
                                    {
                                        _logger.LogInformation("      StickerAttr: Alt={Alt}, Stickerset={StickersetType}", stickerAttr.Alt, stickerAttr.Stickerset?.GetType().Name ?? "null");
                                    }
                                    attributes.Add(attr);
                                }
                            }
                            
                            // Проверяем, что атрибуты есть
                            if (!attributes.Any())
                            {
                                _logger.LogWarning("Document {DocId} for gift {GiftId} has no attributes", document.DocumentId, g.GiftId);
                                skipGift = true;
                            }
                            else
                            {
                                _logger.LogInformation("Document {DocId} validated successfully for gift {GiftId}", document.DocumentId, g.GiftId);
                                sticker = new TDocument
                                {
                                    Id = document.DocumentId,
                                    AccessHash = document.AccessHash,
                                    FileReference = document.FileReference.ToArray(),
                                    Date = document.Date,
                                    MimeType = document.MimeType,
                                    Size = document.Size,
                                    DcId = document.DcId,
                                    Attributes = attributes
                                };

                                if (document.Thumbs != null)
                                {
                                    sticker.Thumbs = new TVector<IPhotoSize>(document.Thumbs.Select(p =>
                                    {
                                        switch (p.Type)
                                         {
                                             case "i":
                                                 return (IPhotoSize)new TPhotoStrippedSize { Type = p.Type, Bytes = p.Bytes };
                                             case "j":
                                                 return new TPhotoPathSize { Type = p.Type, Bytes = p.Bytes };
                                             default:
                                                 return new TPhotoSize
                                                 {
                                                     H = p.H,
                                                     Size = (int)p.Size,
                                                     Type = p.Type,
                                                     W = p.W
                                                 };
                                         }
                                    }));
                                }

                                if (document.VideoThumbs != null)
                                {
                                    sticker.VideoThumbs = new TVector<IVideoSize>(document.VideoThumbs.Select(p => new TVideoSize
                                    {
                                        H = p.H,
                                        W = p.W,
                                        Size = (int)p.Size,
                                        Type = p.Type,
                                        VideoStartTs = p.VideoStartTs
                                    }));
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Document {DocId} not found for gift {GiftId}", g.Sticker.Value, g.GiftId);
                        skipGift = true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load sticker document for gift {GiftId}", g.GiftId);
                    skipGift = true;
                }
            }
            else
            {
                _logger.LogWarning("Gift {GiftId} has no StickerId or it's invalid", g.GiftId);
                skipGift = true;
            }

            if (skipGift || sticker == null)
            {
                _logger.LogWarning("Skipping gift {GiftId} - sticker document not found or invalid (StickerId={StickerId})", g.GiftId, g.Sticker);
                continue;
            }

            // Проверяем ConvertStars
            if (g.ConvertStars <= 0)
            {
                _logger.LogWarning("Skipping gift {GiftId} - invalid ConvertStars ({ConvertStars})", g.GiftId, g.ConvertStars);
                continue;
            }

            // Даты первой и последней продажи
            int? firstSaleDate = g.FirstSaleDate;
            int? lastSaleDate = g.LastSaleDate;
            
            if (g.SoldOut)
            {
                var now = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
                firstSaleDate ??= now;
                lastSaleDate ??= now;
            }
            
            var availabilityRemains = g.AvailabilityRemains;
            var availabilityTotal = g.AvailabilityTotal;
            
            if (g.Limited)
            {
                if (!availabilityRemains.HasValue) availabilityRemains = 0;
                if (!availabilityTotal.HasValue) availabilityTotal = 0;
            }
            else
            {
                availabilityRemains = null;
                availabilityTotal = null;
            }
            
            // Заполняем AvailabilityResale реальным количеством, чтобы клиент показал вкладку перепродажи.
            // Если количество больше нуля, дополнительно считаем среднюю цену по реальным лотам.
            var availabilityResale = resaleCounts.TryGetValue(g.GiftId, out var count) ? count : (long?)null;

            // Считаем среднюю цену по реальным лотам перепродажи.
            // ResellMinStars показывается в клиенте tdesktop как «средняя цена».
            long? resellMinStars = null;
            if (availabilityResale.HasValue && availabilityResale.Value > 0)
            {
                // Берём среднюю цену из выставленных на перепродажу лотов
                var giftsCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
                var resaleFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("GiftId", g.GiftId),
                    Builders<BsonDocument>.Filter.Eq("ForResale", true),
                    Builders<BsonDocument>.Filter.Eq("Upgraded", true)
                );
                
                var resaleGifts = await giftsCollection.Find(resaleFilter).ToListAsync();
                if (resaleGifts.Any())
                {
                    var prices = resaleGifts
                        .Where(doc => doc.Contains("ResaleStars") && !doc["ResaleStars"].IsBsonNull)
                        .Select(doc => doc["ResaleStars"].ToInt64())
                        .ToList();
                    
                    if (prices.Any())
                    {
                        // Считаем среднюю цену по реальным лотам
                        resellMinStars = (long)prices.Average();
                        Console.WriteLine($"[GetStarGifts] Calculated avg resale price for Gift{g.GiftId}: {resellMinStars} stars (from {prices.Count} listings)");
                    }
                    else
                    {
                        // Запасной вариант: если ResaleStars не задан, берём исходную цену, делённую на 8
                        resellMinStars = g.Stars / 8;
                        Console.WriteLine($"[GetStarGifts] Using fallback price for Gift{g.GiftId}: {resellMinStars} stars");
                    }
                }
            }
            
            var gift = new TStarGift
            {
                Id = g.GiftId,
                Limited = g.Limited,
                SoldOut = g.SoldOut,
                Birthday = g.Birthday,
                // Убрано ради совместимости с клиентом 5.16.0
                // RequirePremium = g.RequirePremium,
                // LimitedPerUser = g.LimitedPerUser,
                Sticker = sticker,
                Stars = g.Stars,
                AvailabilityRemains = availabilityRemains,
                AvailabilityTotal = availabilityTotal,
                AvailabilityResale = availabilityResale, // количество лотов перепродажи для клиента
                ConvertStars = g.ConvertStars,
                FirstSaleDate = firstSaleDate,
                LastSaleDate = lastSaleDate,
                UpgradeStars = g.UpgradeStars,
                ResellMinStars = resellMinStars, // задаётся, если перепродажа доступна (для флага 4 нужны оба поля)
                Title = string.IsNullOrWhiteSpace(g.Title) ? null : g.Title,
                // Убрано ради совместимости с клиентом 5.16.0
                // PerUserTotal = g.LimitedPerUser ? g.PerUserTotal : null,
                // PerUserRemains = g.LimitedPerUser ? g.PerUserRemains : null,
                // LockedUntilDate = g.LockedUntilDate,
            };
            
            giftsList.Add(gift);
        }

        var result = new MyTelegram.Schema.Payments.TStarGifts
        {
            Hash = currentHash,
            Gifts = new TVector<IStarGift>(giftsList),
            Chats = new TVector<IChat>(), 
            Users = new TVector<IUser>() 
        };
        
        // Логируем идентификатор конструктора для отладки
        if (giftsList.Count > 0)
        {
            var firstGift = giftsList[0] as TStarGift;
            if (firstGift != null)
            {
                _logger.LogInformation("TStarGift ConstructorId = 0x{ConstructorId:X} (expected 0xc62aca28)", firstGift.ConstructorId);
            }
        }

        _logger.LogInformation("Returning {Count} star gifts to user {UserId} (Hash={Hash})", giftsList.Count, input.UserId, currentHash);
        return result;
    }
    
    /// <summary>
    /// Считает стабильный хеш каталога подарков по алгоритму Telegram.
    /// Хеш меняется, когда подарок добавлен или удалён, изменилась цена, доступность или статус «распродано».
    /// </summary>
    private static int ComputeGiftsHash(List<IAvailableStarGiftReadModel> gifts)
    {
        if (!gifts.Any()) return 0;

        // Сортируем по идентификатору для стабильности результата
        var sorted = gifts.OrderBy(g => g.GiftId).ToList();

        // Формируем строку хеша из ключевых полей
        var parts = new List<string>();
        foreach (var gift in sorted)
        {
            parts.Add($"{gift.GiftId}");
            parts.Add($"{gift.Stars}");
            parts.Add($"{gift.AvailabilityRemains ?? 0}");
            parts.Add(gift.SoldOut ? "1" : "0");
        }
        
        var hashString = string.Join("-", parts);

        // SHA256: берём первые 4 байта как int32
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(hashString));
        return BitConverter.ToInt32(hashBytes, 0);
    }
    
    private static byte[] GenerateFileReference(long documentId)
    {
        var buffer = new byte[8];
        var timestamp = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds());
        
        buffer[0] = (byte)(timestamp >> 24);
        buffer[1] = (byte)(timestamp >> 16);
        buffer[2] = (byte)(timestamp >> 8);
        buffer[3] = (byte)timestamp;
        
        buffer[4] = (byte)(documentId >> 24);
        buffer[5] = (byte)(documentId >> 16);
        buffer[6] = (byte)(documentId >> 8);
        buffer[7] = (byte)documentId;
        
        return buffer;
    }
}
