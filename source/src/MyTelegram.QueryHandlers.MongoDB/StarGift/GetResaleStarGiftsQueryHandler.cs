using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MyTelegram.Queries.StarGift;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema;
using MyTelegram.Schema.Extensions;
using MyTelegram.Schema.Payments;


namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetResaleStarGiftsQueryHandler : IQueryHandler<GetResaleStarGiftsQuery, IResaleStarGifts>
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<GetResaleStarGiftsQueryHandler> _logger;

    public GetResaleStarGiftsQueryHandler(IMongoDatabase database, ILogger<GetResaleStarGiftsQueryHandler> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IResaleStarGifts> ExecuteQueryAsync(GetResaleStarGiftsQuery query,
        CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<StarGiftReadModel>("eventflow-stargiftreadmodel");
        var availableGiftsCollection = _database.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");

        var filterBuilder = Builders<StarGiftReadModel>.Filter;
        var filter = filterBuilder.Eq(x => x.ForResale, true) & filterBuilder.Eq(x => x.Converted, false);

        if (query.GiftId.HasValue && query.GiftId > 0)
        {
            filter &= filterBuilder.Eq(x => x.GiftId, query.GiftId);
        }

        var sort = query.SortByPrice 
            ? Builders<StarGiftReadModel>.Sort.Descending(x => x.ResaleStars)
            : Builders<StarGiftReadModel>.Sort.Descending(x => x.ResaleDate);

        var totalCount = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);

        var gifts = await collection.Find(filter)
            .Sort(sort)
            .Skip(query.Offset)
            .Limit(query.Limit)
            .ToListAsync(cancellationToken);

        var resaleStarGifts = new TVector<IStarGift>();
        var userIds = new HashSet<long>();
        var attributesList = new List<IStarGiftAttribute>();

        foreach (var g in gifts)
        {
            userIds.Add(g.ToUserId); // Текущий владелец (продавец)

            // Определяем документ стикера подарка
            IDocument sticker = new TDocumentEmpty();

            // Формируем уникальный идентификатор подарка
            var uniqueGiftId = g.CollectibleId ?? (Math.Abs((long)g.AggregateId.GetHashCode()) + (g.GiftId * 1000000));

            // Формируем стабильный slug
            var slug = g.UniqueSlug ?? $"Gift{g.GiftId}-{uniqueGiftId}";
            var num = g.UniqueNum ?? (int)(uniqueGiftId % 10000);

            // Десериализуем атрибуты
            var giftAttributes = new TVector<IStarGiftAttribute>();
            if (g.Attributes != null && g.Attributes.Length > 0)
            {
                try
                {
                    var attributesVector = g.Attributes.ToTObject<TVector<IStarGiftAttribute>>();
                    if (attributesVector != null)
                    {
                        giftAttributes = attributesVector;
                        attributesList.AddRange(attributesVector);

                        // Отладочный вывод: подробности по атрибутам
                        _logger.LogInformation("[GetResaleStarGifts] Gift {GiftId} has {Count} attributes:", g.GiftId, attributesVector.Count);
                        foreach (var attr in attributesVector)
                        {
                            switch (attr)
                            {
                                case TStarGiftAttributeModel model:
                                    _logger.LogInformation("   - Model: {Name}, RarityPermille={Rarity}, Document={DocId}", 
                                        model.Name, model.RarityPermille, model.Document?.Id ?? 0);
                                    break;
                                case TStarGiftAttributePattern pattern:
                                    _logger.LogInformation("   - Pattern: {Name}, RarityPermille={Rarity}, Document={DocId}", 
                                        pattern.Name, pattern.RarityPermille, pattern.Document?.Id ?? 0);
                                    break;
                                case TStarGiftAttributeBackdrop backdrop:
                                    _logger.LogInformation("   - Backdrop: {Name}, RarityPermille={Rarity}, CenterColor={Color}", 
                                        backdrop.Name, backdrop.RarityPermille, backdrop.CenterColor);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to deserialize attributes for gift {GiftId}", g.GiftId);
                }
            }
            else
            {
                _logger.LogWarning("[GetResaleStarGifts] Gift {GiftId} has NO attributes in DB (Attributes={Bytes} bytes)",
                    g.GiftId, g.Attributes?.Length ?? 0);
            }

            // Доступность
            var availabilityTotal = 1;
            var availabilityIssued = 1;

            var starGift = new TStarGiftUnique
            {
                Id = uniqueGiftId,
                Title = g.Title ?? $"Unique Gift #{g.GiftId}",
                Slug = slug,
                Num = num,
                OwnerId = new TPeerUser { UserId = g.ToUserId },
                Attributes = giftAttributes,
                AvailabilityIssued = availabilityIssued,
                AvailabilityTotal = availabilityTotal,
                ResellStars = g.ResaleStars
            };
            ((TStarGiftUnique)starGift).ComputeFlag();
            
            resaleStarGifts.Add(starGift);
        }

        // Load user information
        var users = new TVector<IUser>();
        if (userIds.Count > 0)
        {
            var userCollection = _database.GetCollection<UserReadModel>("eventflow-userreadmodel");
            var userFilter = Builders<UserReadModel>.Filter.In(x => x.UserId, userIds);
            var userDocs = await userCollection.Find(userFilter).ToListAsync(cancellationToken);

            foreach (var userDoc in userDocs)
            {
                // Не раскрываем номера телефонов в каталоге перепродажи.
                // Телефон должен быть виден только согласно настройкам приватности,
                // а на маркетплейсе пользователи друг другу незнакомы, поэтому скрываем его по умолчанию.
                users.Add(new TUser
                {
                    Id = userDoc.UserId,
                    AccessHash = userDoc.AccessHash,
                    FirstName = userDoc.FirstName ?? "User",
                    LastName = userDoc.LastName,
                    Username = userDoc.UserName,
                    Phone = null, // Скрываем номер телефона ради приватности
                    Premium = userDoc.Premium
                });
            }
        }
        
        var counters = new TVector<IStarGiftAttributeCounter>();

        return new TResaleStarGifts
        {
            Count = (int)totalCount,
            Gifts = resaleStarGifts,
            Users = users,
            Chats = new TVector<IChat>(), // Пустой список чатов
            Attributes = new TVector<IStarGiftAttribute>(attributesList.DistinctBy(x => 
                x is TStarGiftAttributeModel m ? m.Name : 
                x is TStarGiftAttributePattern p ? p.Name : 
                x is TStarGiftAttributeBackdrop b ? b.Name : x.GetType().Name
            ).ToList()),
            Counters = counters
        };
    }
}

// Вспомогательный класс для ReadModel, если он ещё не доступен (он есть в GetAvailableStarGiftsQueryHandler.cs,
// но при необходимости его придётся сделать публичным или продублировать).
// Предполагаем, что StarGiftCatalogReadModel доступен, либо используем dynamic/BsonDocument.

