using MongoDB.Driver;
using MongoDB.Bson;
using MyTelegram.Schema.Extensions;
using MyTelegram.Schema.Payments;
using MyTelegram.Queries.User;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.getUniqueStarGift" />
///</summary>
internal sealed class GetUniqueStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestGetUniqueStarGift, MyTelegram.Schema.Payments.IUniqueStarGift>,
    Payments.IGetUniqueStarGiftHandler
{
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<GetUniqueStarGiftHandler> _logger;

    public GetUniqueStarGiftHandler(
        IMongoDatabase mongoDatabase,
        IQueryProcessor queryProcessor,
        ILogger<GetUniqueStarGiftHandler> logger)
    {
        _mongoDatabase = mongoDatabase;
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    protected override async Task<MyTelegram.Schema.Payments.IUniqueStarGift> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestGetUniqueStarGift obj)
    {
        _logger.LogInformation("[GetUniqueStarGiftHandler] Request for slug: {Slug}", obj.Slug);

        // Ищем подарок по UniqueSlug
        var collection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
        var filter = Builders<BsonDocument>.Filter.Eq("UniqueSlug", obj.Slug);
        var giftDoc = await collection.Find(filter).FirstOrDefaultAsync();

        // Если по slug ничего не нашли, пробуем разобрать slug и найти подарок по CollectibleId.
        // Это нужно для случаев, когда формат slug поменялся после перепродажи.
        if (giftDoc == null && obj.Slug.Contains("-"))
        {
            var parts = obj.Slug.Split('-');
            if (parts.Length == 2 && long.TryParse(parts[1], out var numericPart))
            {
                // Пробуем найти по CollectibleId (для нового формата вида "Gift2-1570026316")
                var collectibleFilter = Builders<BsonDocument>.Filter.Eq("CollectibleId", numericPart);
                giftDoc = await collection.Find(collectibleFilter).FirstOrDefaultAsync();

                if (giftDoc != null)
                {
                    _logger.LogInformation("[GetUniqueStarGiftHandler] Found gift by CollectibleId: {CollectibleId}", numericPart);
                }
            }
        }

        if (giftDoc == null)
        {
            _logger.LogWarning("[GetUniqueStarGiftHandler] Gift not found with slug: {Slug}", obj.Slug);
            throw new RpcException(RpcErrors.RpcErrors400.GiftSlugInvalid);
        }
        
        var giftId = giftDoc.GetValue("GiftId", 0L).ToInt64();
        var toUserId = giftDoc.GetValue("ToUserId", 0L).ToInt64();
        var titleValue = giftDoc.GetValue("Title", BsonNull.Value);
        var title = titleValue.IsBsonNull ? null : titleValue.AsString;
        var uniqueNum = giftDoc.GetValue("UniqueNum", 1).ToInt32();
        var attributes = giftDoc.Contains("Attributes") && !giftDoc["Attributes"].IsBsonNull 
            ? giftDoc["Attributes"].AsByteArray 
            : null;
        
        // Если Title не задан, подгружаем его из каталога
        if (string.IsNullOrWhiteSpace(title))
        {
            var catalogCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-availablestargiftreadmodel");
            var catalogFilter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
            var catalogDoc = await catalogCollection.Find(catalogFilter).FirstOrDefaultAsync();
            
            if (catalogDoc != null)
            {
                var catalogTitleValue = catalogDoc.GetValue("Title", BsonNull.Value);
                title = catalogTitleValue.IsBsonNull ? null : catalogTitleValue.AsString;
                _logger.LogInformation("[GetUniqueStarGiftHandler] Loaded title from catalog: {Title}", title);
            }
        }

        _logger.LogInformation("[GetUniqueStarGiftHandler] Found gift: GiftId={GiftId}, Owner={OwnerId}, Num={Num}, Title={Title}",
            giftId, toUserId, uniqueNum, title ?? "(null)");

        // Десериализуем атрибуты подарка
        var attributesList = new List<IStarGiftAttribute>();
        if (attributes != null && attributes.Length > 0)
        {
            try
            {
                var attributesVector = attributes.ToTObject<TVector<IStarGiftAttribute>>();
                if (attributesVector != null)
                {
                    attributesList = attributesVector.ToList();
                    _logger.LogInformation("Deserialized {Count} attributes", attributesList.Count);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize attributes");
            }
        }
        
        // Формируем уникальный идентификатор подарка
        var aggregateIdValue = giftDoc.GetValue("AggregateId", BsonNull.Value);
        var aggregateId = aggregateIdValue.IsBsonNull ? "" : aggregateIdValue.AsString;

        // По возможности берём CollectibleId из базы
        var collectibleIdValue = giftDoc.GetValue("CollectibleId", BsonNull.Value);
        var collectibleId = collectibleIdValue.IsBsonNull ? (long?)null : collectibleIdValue.ToInt64();

        var uniqueGiftId = collectibleId ?? (Math.Abs((long)aggregateId.GetHashCode()) + (giftId * 1000000));

        // Создаём TStarGiftUnique
        var uniqueGift = new TStarGiftUnique
        {
            Id = uniqueGiftId,
            Title = title ?? $"Unique Gift #{giftId}",
            Slug = obj.Slug,
            Num = uniqueNum,
            OwnerId = new TPeerUser { UserId = toUserId },
            Attributes = new TVector<IStarGiftAttribute>(attributesList),
            AvailabilityIssued = 1,
            AvailabilityTotal = 1
        };
        uniqueGift.ComputeFlag();
        
        // Загружаем данные владельца подарка
        var ownerUser = await _queryProcessor.ProcessAsync(new GetUserByIdQuery(toUserId));
        var users = new TVector<IUser>();

        if (ownerUser != null)
        {
            // Скрываем номер телефона, если Вы смотрите чужой подарок.
            // Телефон должен быть виден только у Вашего собственного подарка.
            var isOwnGift = toUserId == input.UserId;

            var user = new TUser
            {
                Id = toUserId,
                AccessHash = ownerUser.AccessHash,
                FirstName = ownerUser.FirstName ?? "User",
                LastName = ownerUser.LastName,
                Username = ownerUser.UserName,
                Phone = isOwnGift ? ownerUser.PhoneNumber : null, // скрываем телефон из соображений приватности
                Premium = ownerUser.Premium
            };
            user.ComputeFlag();
            users.Add(user);

            if (!isOwnGift)
            {
                _logger.LogInformation("[PRIVACY] Hiding phone for gift owner {OwnerId} (viewer={ViewerId})", toUserId, input.UserId);
            }
        }
        
        var result = new TUniqueStarGift
        {
            Gift = uniqueGift,
            Users = users
        };
        
        _logger.LogInformation("[GetUniqueStarGiftHandler] Returning gift: {Slug}, Num={Num}", obj.Slug, uniqueNum);
        
        return result;
    }
}
