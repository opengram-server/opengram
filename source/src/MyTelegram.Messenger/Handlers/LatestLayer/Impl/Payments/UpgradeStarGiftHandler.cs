using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Schema;
using MyTelegram.Schema.Payments;
using MongoDB.Driver;
using MongoDB.Bson;
using MyTelegram.Queries.User;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Payments;

///<summary>
/// See <a href="https://corefork.telegram.org/method/payments.upgradeStarGift" />
///</summary>
internal sealed class UpgradeStarGiftHandler : RpcResultObjectHandler<MyTelegram.Schema.Payments.RequestUpgradeStarGift, MyTelegram.Schema.IUpdates>,
    Payments.IUpgradeStarGiftHandler
{
    private readonly ICommandBus _commandBus;
    private readonly IQueryProcessor _queryProcessor;
    private readonly IStarGiftAttributeGenerator _attributeGenerator;
    private readonly IMongoDatabase _mongoDatabase;
    private readonly IIdGenerator _idGenerator;
    private readonly ILogger<UpgradeStarGiftHandler> _logger;
    private readonly IObjectMessageSender _messageSender;
    private readonly IEventBus _eventBus;
    private readonly Services.IDistributedLockService _lockService;

    public UpgradeStarGiftHandler(
        ICommandBus commandBus, 
        IQueryProcessor queryProcessor, 
        IStarGiftAttributeGenerator attributeGenerator,
        IMongoDatabase mongoDatabase,
        IIdGenerator idGenerator,
        ILogger<UpgradeStarGiftHandler> logger,
        IObjectMessageSender messageSender,
        IEventBus eventBus,
        Services.IDistributedLockService lockService)
    {
        _commandBus = commandBus;
        _queryProcessor = queryProcessor;
        _attributeGenerator = attributeGenerator;
        _mongoDatabase = mongoDatabase;
        _idGenerator = idGenerator;
        _logger = logger;
        _messageSender = messageSender;
        _eventBus = eventBus;
        _lockService = lockService;
    }

    protected override async Task<MyTelegram.Schema.IUpdates> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Payments.RequestUpgradeStarGift obj)
    {
        _logger.LogWarning("[UpgradeStarGiftHandler] ===== UPGRADE REQUEST RECEIVED =====");
        _logger.LogWarning("   UserId={UserId}, InputType={InputType}", input.UserId, obj.Stargift?.GetType().Name ?? "null");

        // Шаг 1: определяем идентификатор подарка и загружаем ReadModel (нужен для ключа блокировки)
        string giftInstanceId;
        long giftId;
        int msgId;
        IStarGiftReadModel giftReadModel;
        bool hasPeerContext = false;
        
        if (obj.Stargift is TInputSavedStarGiftUser userGift)
        {
            msgId = userGift.MsgId;
            
            // Загружаем исходный подарок из ReadModel, чтобы получить реальный aggregate ID
            giftReadModel = await _queryProcessor.ProcessAsync(
                new MyTelegram.Queries.StarGift.GetStarGiftByMessageIdQuery(input.UserId, msgId));
            
            if (giftReadModel == null)
            {
                throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
            }
            
            // У старых подарков нет AggregateId, поэтому возвращаем ошибку
            if (string.IsNullOrEmpty(giftReadModel.AggregateId))
            {
                Console.WriteLine($"[UpgradeStarGiftHandler] Gift with Id={giftReadModel.Id} has no AggregateId. This is an old gift that cannot be upgraded.");
                throw new RpcException(RpcErrors.RpcErrors400.MessageIdInvalid);
            }

            giftInstanceId = giftReadModel.AggregateId;
            giftId = giftReadModel.GiftId;

            // Проверяем, не был ли подарок уже улучшен
            if (giftReadModel.Upgraded)
            {
                Console.WriteLine($"[UpgradeStarGiftHandler] Gift {giftInstanceId} is already upgraded!");
                throw new RpcException(new RpcError(400, "GIFT_ALREADY_UPGRADED"));
            }

            // Решаем, какой вариант использовать: peer (исходный чат) или saved (Избранное).
            // Если у подарка есть контекст FromUserId, публикуем в этот чат.
            hasPeerContext = giftReadModel.FromUserId > 0 && giftReadModel.FromUserId != input.UserId;
            Console.WriteLine($"[UpgradeStarGiftHandler] Gift context: FromUserId={giftReadModel.FromUserId}, ToUserId={giftReadModel.ToUserId}, HasPeerContext={hasPeerContext}");
        }
        else
        {
            throw new RpcException(RpcErrors.RpcErrors400.PeerIdInvalid);
        }

        var aggregateId = StarGiftId.Create(giftInstanceId);
        var upgradeDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var upgradeMsgId = await _idGenerator.NextIdAsync(IdType.MessageId, input.UserId);
        
        // Генерируем уникальные атрибуты улучшения.
        // uniqueId — строка, используется как seed для генерации атрибутов;
        // uniqueGiftId далее будет long для TL.
        var uniqueId = $"{giftInstanceId}_{DateTimeOffset.UtcNow.Ticks}";
        var attributes = _attributeGenerator.GenerateUpgradeAttributes(giftId, uniqueId);

        Console.WriteLine($"[UpgradeStarGiftHandler] Generated {attributes.Count} unique attributes for gift {giftId}");

        // Загружаем сведения о подарке из каталога
        var catalogCollection = _mongoDatabase.GetCollection<BsonDocument>("AvailableStarGiftReadModel");
        var catalogFilter = Builders<BsonDocument>.Filter.Eq("GiftId", giftId);
        var catalogDoc = await catalogCollection.Find(catalogFilter).FirstOrDefaultAsync();
        
        int availabilityIssued = 1;
        int availabilityTotal = 1;
        string title = $"Upgraded Gift #{giftId}";
        
        if (catalogDoc != null)
        {
            availabilityIssued = catalogDoc.GetValue("AvailabilityTotal", 1000).ToInt32() - 
                                catalogDoc.GetValue("AvailabilityRemains", 0).ToInt32();
            availabilityTotal = catalogDoc.GetValue("AvailabilityTotal", 1000).ToInt32();
            title = catalogDoc.GetValue("Title", title).AsString;
        }
        
        // Отбираем корректные атрибуты
        var validAttributes = new TVector<IStarGiftAttribute>();
        foreach (var attr in attributes)
        {
            if (attr is TStarGiftAttributeBackdrop backdrop)
            {
                Console.WriteLine($"Adding backdrop attribute: CenterColor={backdrop.CenterColor}, EdgeColor={backdrop.EdgeColor}");
                validAttributes.Add(backdrop);
            }
            else if (attr is TStarGiftAttributeModel model)
            {
                if (model.Document is TDocument modelDoc && !modelDoc.FileReference.IsEmpty && modelDoc.AccessHash != 0)
                {
                    Console.WriteLine($"Adding model attribute: DocumentId={modelDoc.Id}");
                    validAttributes.Add(model);
                }
                else
                {
                    Console.WriteLine($"Skipping model attribute: Document is invalid or empty");
                }
            }
            else if (attr is TStarGiftAttributePattern pattern)
            {
                if (pattern.Document is TDocument patternDoc && !patternDoc.FileReference.IsEmpty && patternDoc.AccessHash != 0)
                {
                    Console.WriteLine($"Adding pattern attribute: DocumentId={patternDoc.Id}");
                    validAttributes.Add(pattern);
                }
                else
                {
                    Console.WriteLine($"Skipping pattern attribute: Document is invalid or empty");
                }
            }
            else
            {
                Console.WriteLine($"Unknown attribute type: {attr?.GetType().Name ?? "null"}");
            }
        }

        Console.WriteLine($"[UpgradeStarGiftHandler] Filtered attributes: {validAttributes.Count} valid out of {attributes.Count} total");

        // Проверяем наличие Model и Pattern (необязательно — клиент может принять подарок только с Backdrop)
        var hasModel = validAttributes.Any(a => a is TStarGiftAttributeModel);
        var hasPattern = validAttributes.Any(a => a is TStarGiftAttributePattern);

        Console.WriteLine($"[UpgradeStarGiftHandler] Attributes check: HasModel={hasModel}, HasPattern={hasPattern}, Total={validAttributes.Count}");

        // Разрешаем улучшение даже без Model/Pattern (используем только Backdrop)
        if (!hasModel || !hasPattern)
        {
            Console.WriteLine($"[UpgradeStarGiftHandler] WARNING: Missing Model/Pattern, using Backdrop-only upgrade");
        }

        // Получаем следующий порядковый номер для этого типа подарка
        var counterCollection = _mongoDatabase.GetCollection<BsonDocument>("StarGiftCounters");
        var counterFilter = Builders<BsonDocument>.Filter.Eq("_id", $"upgraded_{giftId}");
        var counterUpdate = Builders<BsonDocument>.Update.Inc("count", 1);
        var counterOptions = new FindOneAndUpdateOptions<BsonDocument>
        {
            IsUpsert = true,
            ReturnDocument = ReturnDocument.After
        };
        var counterDoc = await counterCollection.FindOneAndUpdateAsync(counterFilter, counterUpdate, counterOptions);
        var sequentialNum = counterDoc["count"].AsInt32;
        
        Console.WriteLine($"[UpgradeStarGiftHandler] Assigned sequential number {sequentialNum} for gift {giftId}");

        // Формируем slug из названия: "Plush Pepe" -> "PlushPepe-1234"
        var cleanTitle = title?.Replace(" ", "") ?? $"Gift{giftId}";
        var slug = $"{cleanTitle}-{sequentialNum}";

        // Создаём TStarGiftUnique
        var uniqueIdHash = giftInstanceId.GetHashCode();
        var uniqueGiftId = Math.Abs((long)uniqueIdHash) + (giftId * 1000000);
        
        var uniqueGift = new TStarGiftUnique
        {
            Id = uniqueGiftId,
            Title = title,
            Slug = slug,
            Num = sequentialNum,
            OwnerId = new TPeerUser { UserId = input.UserId },
            Attributes = validAttributes,  // используем реальные атрибуты, а не пустой вектор
            AvailabilityIssued = availabilityIssued,
            AvailabilityTotal = availabilityTotal
        };

        // ComputeFlag() обязательно нужно вызвать до сериализации
        uniqueGift.ComputeFlag();
        Console.WriteLine($"[UpgradeStarGiftHandler] TStarGiftUnique created: Slug={slug}, Num={sequentialNum}, Flags={uniqueGift.Flags}");

        // Сериализуем атрибуты для хранения в базе (нужно для обоих вариантов)
        byte[]? attributesBytes = null;
        try
        {
            var attributesVector = new TVector<IStarGiftAttribute>(validAttributes);
            attributesBytes = attributesVector.ToBytes();
            Console.WriteLine($"[UpgradeStarGiftHandler] Serialized {validAttributes.Count} attributes to {attributesBytes?.Length ?? 0} bytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UpgradeStarGiftHandler] Failed to serialize attributes: {ex.Message}");
            attributesBytes = null;
        }

        // Выбираем вариант обработки в зависимости от контекста подарка
        if (hasPeerContext)
        {
            // Вариант PEER: отправляем сервисное сообщение в чат с отправителем подарка
            Console.WriteLine($"[UpgradeStarGiftHandler] PEER-variant: creating service message in chat with user {giftReadModel.FromUserId}");

            // Создаём TMessageService для пира (того, кто подарил)
            var peerMessageAction = new TMessageActionStarGiftUnique
            {
                Upgrade = true,
                Saved = false,  // важно: это не saved-вариант
                FromId = new TPeerUser { UserId = input.UserId },
                Peer = new TPeerUser { UserId = giftReadModel.FromUserId },  // отправитель подарка
                SavedId = null,  // важно: в peer-варианте SavedId не задаём
                Gift = uniqueGift
            };
            peerMessageAction.ComputeFlag();
            Console.WriteLine($"[UpgradeStarGiftHandler] PEER: Peer set, SavedId=null, flag bit 7 NOT set");
            
            var peerServiceMessage = new MyTelegram.Schema.TMessageService
            {
                Out = true,  // Outgoing to peer
                Id = (int)upgradeMsgId,
                FromId = new TPeerUser { UserId = input.UserId },
                PeerId = new TPeerUser { UserId = giftReadModel.FromUserId },
                Date = upgradeDate,
                Action = peerMessageAction,
                Silent = false,
                Post = false,
                Legacy = false
            };
            peerServiceMessage.ComputeFlag();
            
            // Создаём обновление для текущего пользователя (self) — тоже peer-вариант для единообразия
            var selfMessageAction = new TMessageActionStarGiftUnique
            {
                Upgrade = true,
                Saved = false,  // для self тоже peer-вариант
                FromId = new TPeerUser { UserId = input.UserId },
                Peer = new TPeerUser { UserId = giftReadModel.FromUserId },
                SavedId = null,
                Gift = uniqueGift
            };
            selfMessageAction.ComputeFlag();
            
            var selfServiceMessage = new MyTelegram.Schema.TMessageService
            {
                Out = true,
                Id = (int)upgradeMsgId,
                FromId = new TPeerUser { UserId = input.UserId },
                PeerId = new TPeerUser { UserId = giftReadModel.FromUserId },
                Date = upgradeDate,
                Action = selfMessageAction,
                Silent = false,
                Post = false,
                Legacy = false
            };
            selfServiceMessage.ComputeFlag();

            Console.WriteLine($"[UpgradeStarGiftHandler] PEER-variant messages created for both users");

            // Получаем PTS для обоих пользователей
            var selfPts = await _idGenerator.NextIdAsync(IdType.Pts, input.UserId, 1);
            var peerPts = await _idGenerator.NextIdAsync(IdType.Pts, giftReadModel.FromUserId, 1);

            // Публикуем команду улучшения
            var command = new UpgradeStarGiftCommand(
                aggregateId,
                input.ToRequestInfo(),
                (int)upgradeMsgId,
                upgradeDate,
                uniqueId,
                attributesBytes
            );
            await _commandBus.PublishAsync(command, CancellationToken.None);
            Console.WriteLine($"[UpgradeStarGiftHandler] Upgrade command published for gift {giftId}");

            // Отправляем интеграционное событие в command-server для создания сообщения
            await _eventBus.PublishAsync(new StarGiftUpgradedIntegrationEvent(
                AggregateId: giftInstanceId,
                FromUserId: giftReadModel.FromUserId,
                ToUserId: input.UserId,
                GiftId: giftId,
                UniqueSlug: slug ?? string.Empty,
                UniqueNum: sequentialNum,
                Attributes: attributesBytes ?? ReadOnlyMemory<byte>.Empty,
                UpgradeDate: upgradeDate,
                UpgradeMsgId: upgradeMsgId
            ));
            Console.WriteLine($"[UpgradeStarGiftHandler] Integration event published for message creation");

            // Обновляем ReadModel полями Title, Slug, Num, CollectibleId, SavedId и CanUpgrade
            // (их нет в событии, поэтому проставляем напрямую)
            var readModelCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
            var readModelFilter = Builders<BsonDocument>.Filter.Eq("_id", giftInstanceId);
            var readModelUpdate = Builders<BsonDocument>.Update
                .Set("Title", title)  // Title нужен для сообщений о передаче и перепродаже
                .Set("UniqueSlug", slug)
                .Set("UniqueNum", sequentialNum)
                .Set("CollectibleId", (long)uniqueGiftId)  // CollectibleId нужен для emoji-статуса
                .Set("SavedId", (long)uniqueGiftId)  // приводим SavedId к значению CollectibleId
                .Set("CanUpgrade", false);  // повторно улучшить уже нельзя
            await readModelCollection.UpdateOneAsync(readModelFilter, readModelUpdate);
            Console.WriteLine($"[UpgradeStarGiftHandler] Updated ReadModel with Title={title}, Slug={slug}, Num={sequentialNum}, CollectibleId={uniqueGiftId}, SavedId={uniqueGiftId}, CanUpgrade=false");

            // Загружаем информацию о пользователях для вектора Users
            var selfUserReadModel = await _queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));
            var peerUserReadModel = await _queryProcessor.ProcessAsync(new GetUserByIdQuery(giftReadModel.FromUserId));
            
            if (selfUserReadModel == null || peerUserReadModel == null)
            {
                throw new RpcException(new RpcError(400, "USER_NOT_FOUND"));
            }
            
            var selfUser = new TUser
            {
                Id = input.UserId,
                AccessHash = selfUserReadModel.AccessHash,
                FirstName = selfUserReadModel.FirstName ?? "User",
                LastName = selfUserReadModel.LastName,
                Username = selfUserReadModel.UserName,
                Phone = selfUserReadModel.PhoneNumber,
                Premium = selfUserReadModel.Premium,  // обязательно проставляем Premium
                Self = true
            };
            selfUser.ComputeFlag();
            
            var peerUser = new TUser
            {
                Id = giftReadModel.FromUserId,
                AccessHash = peerUserReadModel.AccessHash,
                FirstName = peerUserReadModel.FirstName ?? "User",
                LastName = peerUserReadModel.LastName,
                Username = peerUserReadModel.UserName,
                Phone = peerUserReadModel.PhoneNumber,
                Premium = peerUserReadModel.Premium,  // обязательно проставляем Premium
                Self = false
            };
            peerUser.ComputeFlag();

            // Постоянные сообщения в чате создаст StarGiftDomainEventHandler автоматически,
            // когда получит StarGiftUpgradedEvent от command-server
            Console.WriteLine($"[UpgradeStarGiftHandler] PEER-variant completed (permanent chat messages will be created by domain event handler)");

            // Возвращаем обновление для самого пользователя
            var selfUpdate = new TUpdateNewMessage
            {
                Message = selfServiceMessage,
                Pts = (int)selfPts,
                PtsCount = 1
            };
            
            var globalSeqNo = await _idGenerator.NextLongIdAsync(IdType.GlobalSeqNo);
            
            var result = new TUpdates
            {
                Updates = new TVector<IUpdate>(selfUpdate),
                Users = new TVector<IUser>(selfUser, peerUser),  // включаем обоих пользователей
                Chats = new TVector<IChat>(),
                Date = upgradeDate,
                Seq = (int)globalSeqNo
            };

            Console.WriteLine($"[UpgradeStarGiftHandler] PEER-variant completed, returning TUpdates with Seq={globalSeqNo}");
            return result;
        }
        else
        {
            // Вариант SAVED: у подарка нет контекста пира (например, анонимный или от себя)
            Console.WriteLine($"[UpgradeStarGiftHandler] SAVED-variant: creating notification for Saved Messages");

            var messageAction = new TMessageActionStarGiftUnique
            {
                Upgrade = true,
                Saved = true,
                FromId = new TPeerUser { UserId = input.UserId },
                Peer = new TPeerUser { UserId = input.UserId },  // при заданном SavedId поле Peer не должно быть null
                SavedId = msgId,
                Gift = uniqueGift
            };
            messageAction.ComputeFlag();
            Console.WriteLine($"[UpgradeStarGiftHandler] SAVED: Peer AND SavedId both set for flag bit 7");
            Console.WriteLine($"[UpgradeStarGiftHandler] TMessageActionStarGiftUnique flags computed: {messageAction.Flags}");

        // Создаём сервисное сообщение.
        // Out=true, потому что пользователь улучшает собственный подарок (сообщение самому себе)
        var serviceMessage = new MyTelegram.Schema.TMessageService
        {
            Out = true,  // исходящее сообщение самому себе
            Id = (int)upgradeMsgId,
            FromId = new TPeerUser { UserId = input.UserId },
            PeerId = new TPeerUser { UserId = input.UserId },
            Date = upgradeDate,
            Action = messageAction,
            Silent = false,
            Post = false,
            Legacy = false,
            Mentioned = false,
            MediaUnread = false
        };
        
        Console.WriteLine($"[UpgradeStarGiftHandler] TMessageService created: Id={serviceMessage.Id}, Out={serviceMessage.Out}, Date={serviceMessage.Date}");

        // ComputeFlag() обязательно нужно вызвать до сериализации
        serviceMessage.ComputeFlag();

        Console.WriteLine($"[UpgradeStarGiftHandler] TMessageService flags after ComputeFlag: {serviceMessage.Flags}");

        // Сериализуем и выводим TMessageService для отладки
        try
        {
            var msgBytes = serviceMessage.ToBytes();
            Console.WriteLine($"\n[HEX DUMP] TMessageService only: {msgBytes?.Length ?? 0} bytes");
            if (msgBytes != null && msgBytes.Length > 0)
            {
                var displayLength = Math.Min(64, msgBytes.Length);
                Console.Write("First 64 bytes: ");
                for (int i = 0; i < displayLength; i++)
                {
                    Console.Write($"{msgBytes[i]:X2} ");
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to serialize TMessageService: {ex.Message}");
        }

        // Получаем Pts для пользователя
        var newPts = await _idGenerator.NextIdAsync(IdType.Pts, input.UserId, 1);

        // Формируем обновление
        var updateForClient = new TUpdateNewMessage
        {
            Message = serviceMessage,
            Pts = (int)newPts,
            PtsCount = 1
        };

        Console.WriteLine($"[UpgradeStarGiftHandler] Created update with {validAttributes.Count} valid attributes, Pts={newPts}");

        // Загружаем информацию о пользователе для вектора Users (обязательно)
        var userReadModel = await _queryProcessor.ProcessAsync(new GetUserByIdQuery(input.UserId));
        
        if (userReadModel == null)
        {
            throw new RpcException(new RpcError(400, "USER_NOT_FOUND"));
        }
        
        var userForVector = new TUser
        {
            Id = input.UserId,
            AccessHash = userReadModel.AccessHash, // должен быть валидным, не 0
            FirstName = userReadModel.FirstName ?? "User",
            LastName = userReadModel.LastName,
            Username = userReadModel.UserName,
            Phone = userReadModel.PhoneNumber,
            Premium = userReadModel.Premium,  // обязательно проставляем Premium
            Self = true  // это текущий пользователь
        };

        // ComputeFlag() нужно вызвать, чтобы установить бит 0 флага для AccessHash
        userForVector.ComputeFlag();

        Console.WriteLine($"[UpgradeStarGiftHandler] TUser created: UserId={userForVector.Id}, AccessHash={userForVector.AccessHash}, Self={userForVector.Self}, Flags={userForVector.Flags}");

        // Отдельно сериализуем TUser, чтобы проверить корректность
        try
        {
            var userBytes = userForVector.ToBytes();
            Console.WriteLine($"[UpgradeStarGiftHandler] TUser serialized to {userBytes?.Length ?? 0} bytes");
            if (userBytes != null && userBytes.Length > 0)
            {
                Console.Write($"First 32 bytes of TUser: ");
                for (int i = 0; i < Math.Min(32, userBytes.Length); i++)
                {
                    Console.Write($"{userBytes[i]:X2} ");
                }
                Console.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to serialize TUser: {ex.Message}");
        }

        // Публикуем команду для сохранения (асинхронно)
        var command = new UpgradeStarGiftCommand(
            aggregateId,
            input.ToRequestInfo(),
            (int)upgradeMsgId,
            upgradeDate,
            uniqueId,  // строковую версию сохраняем в базе для справки
            attributesBytes
        );

        await _commandBus.PublishAsync(command, CancellationToken.None);

        Console.WriteLine($"[UpgradeStarGiftHandler] Upgrade command published for gift {giftId}");

        // Отправляем интеграционное событие в command-server для создания сообщения (если нужно для saved-варианта).
        // Для saved-варианта сообщения в чате обычно не создаются, так как нет контекста пира,
        // но событие всё равно отправляем — на случай, если обработчик захочет что-то сделать.
        await _eventBus.PublishAsync(new StarGiftUpgradedIntegrationEvent(
            AggregateId: giftInstanceId,
            FromUserId: 0, // в saved-варианте контекста пира нет
            ToUserId: input.UserId,
            GiftId: giftId,
            UniqueSlug: slug,
            UniqueNum: sequentialNum,
            Attributes: attributesBytes ?? ReadOnlyMemory<byte>.Empty,
            UpgradeDate: upgradeDate,
            UpgradeMsgId: upgradeMsgId
        ));
        Console.WriteLine($"[UpgradeStarGiftHandler] Integration event published (saved variant)");

        // Обновляем ReadModel полями Title, Slug, Num, CollectibleId, SavedId и CanUpgrade
        // (их нет в событии, поэтому проставляем напрямую)
        var readModelCollection = _mongoDatabase.GetCollection<BsonDocument>("eventflow-stargiftreadmodel");
        var readModelFilter = Builders<BsonDocument>.Filter.Eq("_id", giftInstanceId);
        var readModelUpdate = Builders<BsonDocument>.Update
            .Set("Title", title)  // Title нужен для сообщений о передаче и перепродаже
            .Set("UniqueSlug", slug)
            .Set("UniqueNum", sequentialNum)
            .Set("CollectibleId", (long)uniqueGiftId)  // CollectibleId нужен для emoji-статуса
            .Set("SavedId", (long)uniqueGiftId)  // приводим SavedId к значению CollectibleId
            .Set("CanUpgrade", false);  // повторно улучшить уже нельзя
        await readModelCollection.UpdateOneAsync(readModelFilter, readModelUpdate);
        Console.WriteLine($"[UpgradeStarGiftHandler] Updated ReadModel with Title={title}, Slug={slug}, Num={sequentialNum}, CollectibleId={uniqueGiftId}, SavedId={uniqueGiftId}, CanUpgrade=false");

        // Получаем GlobalSeqNo (Seq=0 недопустим для TUpdates)
        var globalSeqNo = await _idGenerator.NextLongIdAsync(IdType.GlobalSeqNo);

        // Возвращаем полный ответ: и вектор Updates, и вектор Users
        var result = new TUpdates
        {
            Updates = new TVector<IUpdate>(updateForClient), // добавляем обновление
            Users = new TVector<IUser>(userForVector),       // добавляем пользователя
            Chats = new TVector<IChat>(),                    // пусто
            Date = upgradeDate,
            Seq = (int)globalSeqNo
        };

        Console.WriteLine($"[UpgradeStarGiftHandler] Full TUpdates with Updates + Users");
        Console.WriteLine($"[UpgradeStarGiftHandler] Returning TUpdates: Seq={globalSeqNo}, Updates={result.Updates.Count}, Users={result.Users.Count}");

        // Дамп в HEX для отладки
        try
        {
            var resultBytes = result.ToBytes();
            Console.WriteLine($"\n[HEX DUMP] Serialized TUpdates: {resultBytes?.Length ?? 0} bytes");
            if (resultBytes != null && resultBytes.Length > 0)
            {
                var displayLength = Math.Min(512, resultBytes.Length);
                Console.WriteLine($"First {displayLength} bytes:");
                for (int i = 0; i < displayLength; i += 16)
                {
                    Console.Write($"{i:D4}: ");
                    for (int j = 0; j < 16 && i + j < displayLength; j++)
                    {
                        Console.Write($"{resultBytes[i + j]:X2} ");
                    }
                    Console.WriteLine();
                }
                
                // Показываем расположение ключевых структур
                Console.WriteLine($"\nStructure breakdown:");
                Console.WriteLine($"  TUpdates constructor at byte 0");
                Console.WriteLine($"  Updates vector at byte 4");
                Console.WriteLine($"  Users vector should be at byte ~{4 + 4 + 4 + 472 + 8}");
                Console.WriteLine($"  Total length: {resultBytes.Length}");
            }

            // Дополнительно выводим только TStarGiftUnique
            var giftBytes = uniqueGift.ToBytes();
            Console.WriteLine($"\n[HEX DUMP] TStarGiftUnique only: {giftBytes?.Length ?? 0} bytes");
            if (giftBytes != null && giftBytes.Length > 0)
            {
                var displayLength = Math.Min(128, giftBytes.Length);
                for (int i = 0; i < displayLength; i += 16)
                {
                    Console.Write($"{i:D4}: ");
                    for (int j = 0; j < 16 && i + j < displayLength; j++)
                    {
                        Console.Write($"{giftBytes[i + j]:X2} ");
                    }
                    Console.WriteLine();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[HEX DUMP] Failed: {ex.Message}");
        }

        return result;
        } // конец ветки else (saved-вариант)
    }
}
