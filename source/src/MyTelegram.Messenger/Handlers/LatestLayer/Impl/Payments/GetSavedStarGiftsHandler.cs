using MyTelegram.Schema.Payments;
using MyTelegram.Schema.Extensions;
using MyTelegram.Queries.User;
using MongoDB.Driver;
using MongoDB.Bson;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getSavedStarGifts" />
///</summary>
internal sealed class GetSavedStarGiftsHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetSavedStarGifts, MyTelegram.Schema.Payments.ISavedStarGifts>,
    Payments.IGetSavedStarGiftsHandler
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly IPeerHelper _peerHelper;
    private readonly ILogger<GetSavedStarGiftsHandler> _logger;
    private readonly IMongoDatabase _mongoDatabase;

    public GetSavedStarGiftsHandler(
        IQueryProcessor queryProcessor, 
        IPeerHelper peerHelper, 
        ILogger<GetSavedStarGiftsHandler> logger,
        IMongoDatabase mongoDatabase)
    {
        _queryProcessor = queryProcessor;
        _peerHelper = peerHelper;
        _logger = logger;
        _mongoDatabase = mongoDatabase;
    }

    protected override async Task<MyTelegram.Schema.Payments.ISavedStarGifts> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetSavedStarGifts obj)
    {
        Console.WriteLine("[GetSavedStarGiftsHandler] REQUEST RECEIVED!");
        _logger.LogWarning("[GetSavedStarGiftsHandler] UserId={UserId}, Offset={Offset}, Limit={Limit}, Flags={Flags}",
            input.UserId, obj.Offset, obj.Limit, obj.Flags);

        var peer = _peerHelper.GetPeer(obj.Peer, input.UserId);
        _logger.LogWarning("[GetSavedStarGiftsHandler] RequestingUserId={UserId}, TargetPeerId={PeerId}", input.UserId, peer.PeerId);

        // При просмотре чужого профиля peer.PeerId — это пользователь, чьи подарки мы хотим увидеть.
        // При просмотре своего профиля peer.PeerId совпадает с input.UserId.
        var targetUserId = peer.PeerId;

        _logger.LogWarning("[GetSavedStarGiftsHandler] Fetching gifts for TargetUserId={TargetUserId}", targetUserId);

        var savedGifts = await _queryProcessor.ProcessAsync(new GetSavedStarGiftsQuery(
            targetUserId,  // пользователь, чьи подарки запрашиваем (свои или чужие)
            null,          // фильтр по peer здесь не используется
            obj.ExcludeUnsaved,
            obj.ExcludeSaved,
            obj.ExcludeLimited,
            obj.ExcludeUnlimited,
            obj.ExcludeUnique,
            string.IsNullOrEmpty(obj.Offset) ? 0 : int.Parse(obj.Offset),
            obj.Limit
        ));

        var gifts = new List<ISavedStarGift>();
        
        foreach (var g in savedGifts)
        {
            TDocument? sticker = null;
            if (g.StickerDocumentId.HasValue && g.StickerDocumentId.Value > 0)
            {
                try
                {
                    var document = await _queryProcessor.ProcessAsync(new GetDocumentByIdQuery(g.StickerDocumentId.Value));
                    if (document != null)
                    {
                        var attributes = new List<IDocumentAttribute>();
                        
                        if (document.Attributes2 != null)
                        {
                            attributes.AddRange(document.Attributes2);
                        }
                        
                        sticker = new TDocument
                        {
                            Id = document.DocumentId,
                            AccessHash = document.AccessHash,
                            FileReference = document.FileReference.IsEmpty ? Array.Empty<byte>() : document.FileReference.ToArray(),
                            Date = document.Date,
                            MimeType = document.MimeType ?? "application/x-tgsticker",
                            Size = document.Size,
                            DcId = document.DcId,
                            Attributes = new TVector<IDocumentAttribute>(attributes)
                        };
                        
                        _logger.LogInformation("Loaded sticker document {DocumentId} for gift {GiftId}: Size={Size}, DcId={DcId}",
                            document.DocumentId, g.GiftId, document.Size, document.DcId);
                    }
                    else
                    {
                        _logger.LogWarning("Sticker document {DocumentId} not found for gift {GiftId}", g.StickerDocumentId.Value, g.GiftId);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load sticker document {DocumentId} for gift {GiftId}: {Error}",
                        g.StickerDocumentId.Value, g.GiftId, ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Gift {GiftId} has no sticker document ID", g.GiftId);
            }

            if (sticker == null)
            {
                _logger.LogWarning("Creating empty sticker for gift {GiftId}", g.GiftId);
                sticker = new TDocument
                {
                    Id = 0,
                    AccessHash = 0,
                    FileReference = Array.Empty<byte>(),
                    Date = 0,
                    MimeType = "application/x-tgsticker",
                    Size = 0,
                    DcId = 0,
                    Attributes = new TVector<IDocumentAttribute>()
                };
            }
            
            // Создаём TStarGift или TStarGiftUnique в зависимости от флага Upgraded
            IStarGift gift;

            // Подгружаем данные из каталога, чтобы поля limited/availability были согласованы
            var catalogCollection = _mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
            var catalogFilter = Builders<BsonDocument>.Filter.Eq("GiftId", g.GiftId);
            var catalogDoc = await catalogCollection.Find(catalogFilter).FirstOrDefaultAsync();

            static int? GetNullableInt(BsonDocument? doc, string name)
            {
                if (doc == null || !doc.TryGetValue(name, out var value) || value.IsBsonNull) return null;
                if (value.IsInt32) return value.AsInt32;
                if (value.IsInt64) return (int)value.AsInt64;
                return null;
            }

            static long? GetNullableLong(BsonDocument? doc, string name)
            {
                if (doc == null || !doc.TryGetValue(name, out var value) || value.IsBsonNull) return null;
                if (value.IsInt64) return value.AsInt64;
                if (value.IsInt32) return value.AsInt32;
                return null;
            }

            static bool GetBool(BsonDocument? doc, string name, bool defaultValue = false)
            {
                if (doc == null || !doc.TryGetValue(name, out var value) || value.IsBsonNull) return defaultValue;
                return value.ToBoolean();
            }

            var catalogTitle = catalogDoc?.GetValue("Title", BsonNull.Value).IsBsonNull == false
                ? catalogDoc.GetValue("Title").AsString
                : g.Title;

            var catalogLimited = GetBool(catalogDoc, "Limited");
            var catalogSoldOut = GetBool(catalogDoc, "SoldOut");
            var catalogBirthday = GetBool(catalogDoc, "Birthday");
            var catalogAvailabilityRemains = GetNullableInt(catalogDoc, "AvailabilityRemains");
            var catalogAvailabilityTotal = GetNullableInt(catalogDoc, "AvailabilityTotal");
            var catalogAvailabilityResale = GetNullableLong(catalogDoc, "AvailabilityResale");
            var catalogResellMinStars = GetNullableLong(catalogDoc, "ResellMinStars");
            
            if (g.Upgraded)
            {
                // Подарок улучшён — возвращаем TStarGiftUnique
                _logger.LogInformation("[GetSavedStarGiftsHandler] Gift {GiftId} is upgraded, creating TStarGiftUnique", g.GiftId);

                // Десериализуем атрибуты из базы
                var attributesList = new List<IStarGiftAttribute>();
                if (g.Attributes != null && g.Attributes.Length > 0)
                {
                    try
                    {
                        var attributesVector = g.Attributes.ToTObject<TVector<IStarGiftAttribute>>();
                        if (attributesVector != null)
                        {
                            attributesList = attributesVector.ToList();
                            _logger.LogInformation("[GetSavedStarGiftsHandler] Deserialized {Count} attributes for upgraded gift", attributesList.Count);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "[GetSavedStarGiftsHandler] Failed to deserialize attributes for gift {GiftId}", g.GiftId);
                    }
                }

                // Формируем уникальный идентификатор на основе AggregateId
                var uniqueIdHash = g.AggregateId.GetHashCode();
                var uniqueGiftId = Math.Abs((long)uniqueIdHash) + (g.GiftId * 1000000);

                _logger.LogInformation("[GetSavedStarGiftsHandler] Calculated CollectibleId for gift {AggregateId}: uniqueGiftId={UniqueGiftId}, hash={Hash}, GiftId={GiftId}",
                    g.AggregateId, uniqueGiftId, uniqueIdHash, g.GiftId);

                string title = catalogTitle ?? $"Unique Gift #{g.GiftId}";
                int availabilityTotal = catalogAvailabilityTotal ?? 1;

                // Используем сохранённые Slug и Num, если они есть, иначе подставляем значения по умолчанию
                var slug = g.UniqueSlug ?? $"upgraded_{g.GiftId}_{Math.Abs(uniqueIdHash)}";
                var num = g.UniqueNum ?? 1;

                // AvailabilityIssued — сколько подарков этого типа уже улучшено
                var giftsCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
                var countFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("GiftId", g.GiftId),
                    Builders<BsonDocument>.Filter.Eq("Upgraded", true)
                );
                var upgradedCount = (int)await giftsCollection.CountDocumentsAsync(countFilter);
                
                gift = new TStarGiftUnique
                {
                    Id = uniqueGiftId,
                    Title = title,
                    Slug = slug,
                    Num = num,
                    OwnerId = new TPeerUser { UserId = g.ToUserId },
                    Attributes = new TVector<IStarGiftAttribute>(attributesList),
                    AvailabilityIssued = upgradedCount,
                    AvailabilityTotal = availabilityTotal,
                    ResellStars = g.ForResale ? g.ResaleStars : null  // цена перепродажи, если подарок выставлен
                };
                ((TStarGiftUnique)gift).ComputeFlag();

                _logger.LogInformation("[GetSavedStarGiftsHandler] Created TStarGiftUnique: Slug={Slug}, Num={Num}/{Total}, Title={Title}, CanUpgrade={CanUpgrade}, ForResale={ForResale}, ResaleStars={ResaleStars}, ResellStarsSet={ResellStarsSet}",
                    slug, num, availabilityTotal, title, g.CanUpgrade, g.ForResale, g.ResaleStars, ((TStarGiftUnique)gift).ResellStars);
            }
            else
            {
                // Обычный подарок — возвращаем TStarGift
                gift = new TStarGift
                {
                    Id = g.GiftId,
                    Limited = catalogLimited,
                    SoldOut = catalogSoldOut,
                    Birthday = catalogBirthday,
                    Sticker = sticker,
                    Stars = g.Stars,
                    AvailabilityRemains = catalogAvailabilityRemains,
                    AvailabilityTotal = catalogAvailabilityTotal,
                    AvailabilityResale = catalogAvailabilityResale,
                    ConvertStars = g.ConvertStars,
                    UpgradeStars = g.UpgradeStars,
                    ResellMinStars = catalogResellMinStars,
                    Title = catalogTitle ?? g.Title ?? ""
                };
            }
            
            var savedGift = new TSavedStarGift
            {
                NameHidden = g.NameHidden,
                Unsaved = !g.Saved,
                Refunded = g.Refunded,
                CanUpgrade = g.CanUpgrade && !g.Upgraded, // нельзя улучшить уже улучшённый подарок
                UpgradeSeparate = g.CanUpgrade && !g.Upgraded,
                PinnedToTop = g.Pinned,
                FromId = g.FromUserId != 0 ? new TPeerUser { UserId = g.FromUserId } : null,
                Date = g.Date,
                Gift = gift,
                Message = !string.IsNullOrEmpty(g.Message) ? new TTextWithEntities 
                { 
                    Text = g.Message,
                    Entities = new TVector<IMessageEntity>()
                } : null,
                MsgId = g.MessageId,
                // SavedId нужен клиентам, чтобы выполнить улучшение из профиля. Для улучшённых подарков используем
                // CollectibleId (starGiftUnique.id). Для неулучшённых SavedId может быть null, тогда берём id сообщения.
                SavedId = g.CollectibleId ?? g.SavedId ?? (long)g.MessageId,
                ConvertStars = g.Converted ? g.ConvertStars : null,
                UpgradeStars = g.UpgradeStars,
                CanResellAt = g.ForResale ? 0 : (g.Upgraded ? CurrentDate : null) // 0 — уже выставлен на продажу, CurrentDate — можно перепродать сейчас, null — перепродажа невозможна
            };

            _logger.LogInformation("Gift {Index}: GiftId={GiftId}, FromUserId={FromUserId}, Stars={Stars}, StickerDocId={StickerDocId}, MsgId={MsgId}, ForResale={ForResale}, CanResellAt={CanResellAt}",
                gifts.Count + 1, g.GiftId, g.FromUserId, g.Stars, g.StickerDocumentId, g.MessageId, g.ForResale, savedGift.CanResellAt);
            
            gifts.Add(savedGift);
        }

        var count = savedGifts.Count;
        var hasMore = count >= obj.Limit;

        // Загружаем данные всех упомянутых пользователей (отправителей и получателей)
        var userIds = new HashSet<long>();
        userIds.Add(peer.PeerId); // владелец (получатель)
        foreach (var g in savedGifts)
        {
            if (g.FromUserId != 0)
            {
                userIds.Add(g.FromUserId); // отправитель
            }
        }
        
        var users = new List<IUser>();
        foreach (var userId in userIds)
        {
            try
            {
                var userReadModel = await _queryProcessor.ProcessAsync(new GetUserByIdQuery(userId));
                if (userReadModel != null)
                {
                    // Скрываем номер телефона при просмотре чужого профиля.
                    // Телефон должен показываться только согласно настройкам приватности,
                    // а при просмотре чужих подарков мы по умолчанию не считаемся контактом.
                    var shouldHidePhone = userId != input.UserId; // скрываем телефон при просмотре чужого профиля

                    var user = new TUser
                    {
                        Id = userId,
                        AccessHash = userReadModel.AccessHash,
                        FirstName = userReadModel.FirstName ?? "User",
                        LastName = userReadModel.LastName,
                        Username = userReadModel.UserName,
                        Phone = shouldHidePhone ? null : userReadModel.PhoneNumber, // скрываем телефон из соображений приватности
                        Premium = userReadModel.Premium
                    };
                    user.ComputeFlag();
                    users.Add(user);

                    if (shouldHidePhone)
                    {
                        _logger.LogInformation("[PRIVACY] Hiding phone for user {UserId} (viewer={ViewerId})", userId, input.UserId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Failed to load user {UserId}: {Error}", userId, ex.Message);
            }
        }
        
        _logger.LogInformation("Returning {Count} saved gifts with {UserCount} users", count, users.Count);

        // Считаем смещение для следующей страницы
        string? nextOffset = null;
        if (hasMore)
        {
            var currentOffset = string.IsNullOrEmpty(obj.Offset) ? 0 : int.Parse(obj.Offset);
            nextOffset = (currentOffset + obj.Limit).ToString();
        }

        var result = new TSavedStarGifts
        {
            Count = count,
            ChatNotificationsEnabled = null, // необязательное поле — если не задано, значения нет
            Gifts = new TVector<ISavedStarGift>(gifts ?? new List<ISavedStarGift>()),
            NextOffset = nextOffset,
            Chats = new TVector<IChat>(),
            Users = new TVector<IUser>(users) // список пользователей не пустой
        };

        _logger.LogWarning("[GetSavedStarGiftsHandler] RETURNING: Count={Count}, GiftsCount={GiftsCount}, UsersCount={UsersCount}, NextOffset={NextOffset}",
            result.Count, result.Gifts.Count, result.Users.Count, result.NextOffset);
        
        return result;
    }
}
