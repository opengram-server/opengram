using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MyTelegram.Queries.StarGift;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema;
using MyTelegram.Schema.Extensions;
using MyTelegram.Schema.Payments;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetUserStarGiftsQueryHandler : IQueryHandler<GetUserStarGiftsQuery, IUserStarGifts>
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<GetUserStarGiftsQueryHandler> _logger;

    public GetUserStarGiftsQueryHandler(IMongoDatabase database, ILogger<GetUserStarGiftsQueryHandler> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<IUserStarGifts> ExecuteQueryAsync(GetUserStarGiftsQuery query, CancellationToken cancellationToken)
    {
        var collection = _database.GetCollection<StarGiftReadModel>("eventflow-stargiftreadmodel");
        
        var filterBuilder = Builders<StarGiftReadModel>.Filter;
        // Показываем только сохранённые подарки: у скрытых Saved=false, в профиле их быть не должно.
        // Также исключаем подарки, конвертированные в звёзды.
        var filter = filterBuilder.Eq(x => x.ToUserId, query.UserId) &
                     filterBuilder.Eq(x => x.Saved, true) & // Только сохранённые (нескрытые) подарки
                     filterBuilder.Eq(x => x.Converted, false); // Не показываем конвертированные подарки

        var sort = Builders<StarGiftReadModel>.Sort.Descending(x => x.Date);

        // Считаем общее количество без учёта лимита, чтобы вернуть точное число
        var totalCount = await collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        
        var gifts = await collection.Find(filter)
            .Sort(sort)
            .Limit(query.Limit)
            .ToListAsync(cancellationToken);

        var userStarGifts = new TVector<IUserStarGift>();

        // Собираем все ID пользователей, упомянутых в подарках (отправители и получатель)
        var userIds = new HashSet<long> { query.UserId }; // Владелец (получатель)
        foreach (var g in gifts)
        {
            if (g.FromUserId != 0)
            {
                userIds.Add(g.FromUserId); // Отправитель
            }
        }

        // Загружаем данные пользователей
        var users = new TVector<IUser>();
        var userCollection = _database.GetCollection<UserReadModel>("eventflow-userreadmodel");
        var userFilter = Builders<UserReadModel>.Filter.In(x => x.UserId, userIds);
        var userDocs = await userCollection.Find(userFilter).ToListAsync(cancellationToken);
        
        foreach (var userDoc in userDocs)
        {
            users.Add(new TUser
            {
                Id = userDoc.UserId,
                AccessHash = userDoc.AccessHash,
                FirstName = userDoc.FirstName ?? "User",
                LastName = userDoc.LastName,
                Username = userDoc.UserName,
                Phone = userDoc.PhoneNumber
            });
        }

        // При необходимости заранее загружаем документы
        var stickerIds = gifts.Where(g => g.StickerDocumentId.HasValue).Select(g => g.StickerDocumentId!.Value).Distinct().ToList();
        var documents = new Dictionary<long, DocumentReadModel>();
        
        if (stickerIds.Any())
        {
            var docCollection = _database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
            var docFilter = Builders<DocumentReadModel>.Filter.In(x => x.DocumentId, stickerIds);
            var docs = await docCollection.Find(docFilter).ToListAsync(cancellationToken);
            documents = docs.ToDictionary(d => d.DocumentId, d => d);
        }

        foreach (var gift in gifts)
        {
            // Определяем документ стикера подарка
            IDocument sticker = new TDocumentEmpty();
            if (gift.StickerDocumentId.HasValue && documents.TryGetValue(gift.StickerDocumentId.Value, out var doc))
            {
                 sticker = new TDocument
                 {
                     Id = doc.DocumentId,
                     AccessHash = doc.AccessHash,
                     FileReference = doc.FileReference.ToArray(),
                     Date = doc.Date,
                     MimeType = doc.MimeType,
                     Size = doc.Size,
                     DcId = doc.DcId,
                     Attributes = new TVector<IDocumentAttribute>(doc.Attributes2 ?? new List<IDocumentAttribute>()),
                     Thumbs = new TVector<IPhotoSize>() // Пока без миниатюр
                 };
            }

            // В зависимости от флага Upgraded возвращаем TStarGift или TStarGiftUnique
            IStarGift starGift;

            if (gift.Upgraded)
            {
                // Подарок улучшен — возвращаем TStarGiftUnique.
                // Десериализуем атрибуты из базы.
                var attributesList = new List<IStarGiftAttribute>();
                if (gift.Attributes != null && gift.Attributes.Length > 0)
                {
                    try
                    {
                        var attributesVector = gift.Attributes.ToTObject<TVector<IStarGiftAttribute>>();
                        if (attributesVector != null)
                        {
                            attributesList = attributesVector.ToList();
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "[GetUserStarGiftsQueryHandler] Failed to deserialize attributes for gift {GiftId}", gift.GiftId);
                    }
                }

                // Формируем уникальный идентификатор подарка.
                // Если в базе есть CollectibleId, используем его (он стабилен), иначе считаем по хэшу
                // (значение нестабильно, но нужно для старых записей).
                var uniqueGiftId = gift.CollectibleId ?? (Math.Abs((long)gift.AggregateId.GetHashCode()) + (gift.GiftId * 1000000));

                // Берём сохранённые Slug и Num, если они есть, иначе подставляем значения по умолчанию
                var slug = gift.UniqueSlug ?? $"upgraded_{gift.GiftId}_{uniqueGiftId}";
                var num = gift.UniqueNum ?? 1;
                
                starGift = new TStarGiftUnique
                {
                    Id = uniqueGiftId,
                    Title = gift.Title ?? $"Unique Gift #{gift.GiftId}",
                    Slug = slug,
                    Num = num,
                    OwnerId = new TPeerUser { UserId = gift.ToUserId },
                    Attributes = new TVector<IStarGiftAttribute>(attributesList),
                    AvailabilityIssued = 1,
                    AvailabilityTotal = 1,
                    ResellStars = gift.ForResale ? gift.ResaleStars : null  // Цена перепродажи, если подарок выставлен
                };
                ((TStarGiftUnique)starGift).ComputeFlag();
            }
            else
            {
                // Обычный подарок — возвращаем TStarGift
                starGift = new TStarGift
                {
                    Id = gift.GiftId,
                    Sticker = sticker,
                    Stars = gift.Stars,
                    ConvertStars = gift.ConvertStars,
                    UpgradeStars = gift.UpgradeStars,
                    Title = gift.Title,
                    Limited = false,
                    SoldOut = false,
                    Birthday = false,
                    AvailabilityRemains = null,
                    AvailabilityTotal = null,
                    AvailabilityResale = null,
                    FirstSaleDate = null,
                    LastSaleDate = null,
                    ResellMinStars = null
                };
            }

            var userStarGift = new TUserStarGift
            {
                Date = gift.Date,
                Gift = starGift,
                Message = gift.Message != null ? new TTextWithEntities { Text = gift.Message, Entities = new TVector<IMessageEntity>() } : null,
                MsgId = gift.MessageId,
                SavedId = gift.SavedId,
                ConvertStars = gift.ConvertStars,
                UpgradeStars = gift.UpgradeStars,
                NameHidden = gift.NameHidden,
                Unsaved = !gift.Saved,
                Refunded = gift.Refunded,
                CanUpgrade = gift.CanUpgrade && !gift.Upgraded, // Уже улучшенный подарок улучшить нельзя
                PinnedToTop = gift.Pinned,
                TransferStars = null,
                CanExportAt = null,
                CanTransferAt = null,
                CanResellAt = null
            };
            
            userStarGifts.Add(userStarGift);
        }
        
        return new TUserStarGifts
        {
            Count = (int)totalCount, // Используем общее число из базы, а не только загруженные подарки
            Gifts = userStarGifts,
            Users = users
        };
    }
}
