using System.Linq.Expressions;
using EventFlow.MongoDB.Extensions;
using EventFlow.ReadStores;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using MyTelegram.Domain.Aggregates.AppCode;
using MyTelegram.Domain.Aggregates.Updates;
using MyTelegram.Domain.CommandHandlers.RpcResult;
using MyTelegram.Domain.CommandHandlers.Updates;
using MyTelegram.Domain.Commands.RpcResult;
using MyTelegram.Domain.Commands.Updates;
using MyTelegram.Domain.Events.RpcResult;
using MyTelegram.Domain.Events.Updates;
using MyTelegram.EventFlow.MongoDB.Extensions;
using MyTelegram.EventFlow.MongoDB.ReadStores;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.Messenger.Extensions;
using MyTelegram.Messenger.QueryServer.EventHandlers;
using MyTelegram.EventBus.Extensions;
using MyTelegram.Messenger.QueryServer.Handlers;
using MyTelegram.Messenger.QueryServer.DomainEventHandlers;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.Domain.Events.Stars;

namespace MyTelegram.Messenger.QueryServer.Extensions;

public static class MyTelegramMessengerQueryServerExtensions
{
    public static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddSubscription<MessengerQueryDataReceivedEvent, MessengerEventHandler>();
        services.AddSubscription<StickerDataReceivedEvent, MessengerEventHandler>();

        services.AddSubscription<UserIsOnlineEvent, UserIsOnlineEventHandler>();

        services.AddSubscription<DomainEventMessage, DistributedDomainEventHandler>();
        services.AddSubscription<DuplicateCommandEvent, DuplicateOperationExceptionHandler>();
        
        // Перехватываем GetCustomEmojiDocuments до того, как запрос уйдёт на file-server
        services.AddSubscription<DownloadDataReceivedEvent, GetCustomEmojiDocumentsDownloadHandler>();

        // Обработчик доменных событий по звёздам
        services.AddTransient<StarsDomainEventHandler>();

        // Обработчик сохраняет установленные наборы стикеров в MongoDB
        services.AddTransient<StickerSetInstalledEventHandler>();
        
    }

    public static void AddMyTelegramMessengerQueryServer(this IServiceCollection services, Action<IEventFlowOptions>? configure = null)
    {
        services.AddMyTelegramMessengerServices();
        services.RegisterServices(typeof(HandlerHelper).Assembly);
            
        // Регистрируем собственные хранилища read-моделей
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.DocumentReadModel>, DocumentReadModelStore>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StickerSetReadModel>, StickerSetReadModelStore>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerifierReadModel>, BotVerifierReadModelStore>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerificationReadModel>, BotVerificationReadModelStore>();
        services.AddEventFlow(options =>
        {
            options.AddMyTelegramMongoDbReadModel();
            options.AddQueryHandlers();
            options.AddEvents(typeof(AppCodeAggregate).Assembly);
            options.AddEventUpgraders();
            options.AddMongoDbQueryHandlers();
            
            // EventStore и SnapshotStore на MongoDB (нужны для MongoDbHighValueGenerator)
            options.UseMongoDbEventStore();
            options.UseMongoDbSnapshotStore();

            // Регистрируем QueryHandler-ы из сборки Messenger
            options.AddQueryHandlers(typeof(MyTelegram.Messenger.QueryHandlers.GetContactListBySelfUserIdQueryHandler));
            options.AddQueryHandlers(typeof(MyTelegram.Messenger.QueryHandlers.GetChannelsByIdsQueryHandler));
            options.AddQueryHandlers(typeof(MyTelegram.Messenger.QueryHandlers.GetChannelMemberByUserIdListQueryHandler));
            options.AddSubscribers(Assembly.GetEntryAssembly());
            
            // Подписчик на установку наборов стикеров
            options.AddSubscribers(typeof(StickerSetInstalledEventHandler));

            // Подписчик на доменные события по звёздам
            options.AddSubscribers(typeof(StarsDomainEventHandler));

            options.AddCommands(
                typeof(CreateRpcResultCommand),
                typeof(CreateUpdatesCommand),
                typeof(DeleteRpcResultCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.SaveStarGiftCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.TransferStarGiftCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.UpgradeStarGiftCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.UpdateStarGiftPriceCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.ListStarGiftForResaleCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.RemoveStarGiftFromResaleCommand),
                typeof(MyTelegram.Domain.Commands.StarGift.BuyStarGiftFromResaleCommand)
                );
            options.AddCommandHandlers(
                typeof(CreateRpcResultCommandHandler),
                typeof(CreateUpdatesCommandHandler),
                typeof(DeleteRpcResultCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.User.InstallStickerSetCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.User.UpdateUserPasswordStatusCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.UserPassword.SetPasswordCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.UserPassword.RemovePasswordCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.UserPassword.CreateSrpSessionCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.UserPassword.VerifyPasswordCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.SaveStarGiftCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.TransferStarGiftCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.UpgradeStarGiftCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.UpdateStarGiftPriceCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.ListStarGiftForResaleCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.RemoveStarGiftFromResaleCommandHandler),
                typeof(MyTelegram.Domain.CommandHandlers.StarGift.BuyStarGiftFromResaleCommandHandler)
            );
            options.AddEvents(
                typeof(RpcResultCreatedEvent),
                typeof(UpdatesCreatedEvent),
                typeof(RpcResultDeletedEvent),
                typeof(MyTelegram.Domain.Events.User.UserPasswordStatusUpdatedEvent),
                typeof(MyTelegram.Domain.Events.UserPassword.PasswordSetEvent),
                typeof(MyTelegram.Domain.Events.UserPassword.PasswordRemovedEvent),
                typeof(MyTelegram.Domain.Events.UserPassword.SrpSessionCreatedEvent),
                typeof(MyTelegram.Domain.Events.UserPassword.PasswordVerifiedEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftSavedEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftTransferredEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftUpgradedEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftListedForResaleEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftRemovedFromResaleEvent),
                typeof(MyTelegram.Domain.Events.StarGift.StarGiftSoldViaResaleEvent)
            );
        });
        services.AddEventStoreMongoDbContext();
        services.AddReadModelMongoDbContext();
        
        // Дополнительные read-only хранилища read-моделей
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.PrivacyReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.PrivacyReadModel>>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.InstalledStickerSetReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.InstalledStickerSetReadModel>>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.MongoDB.StarsReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.MongoDB.StarsReadModel>>();
        // Версия Impl нужна для обратной совместимости с GetStarsTransactionsQueryHandler
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StarsReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StarsReadModel>>();
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StarGiftReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StarGiftReadModel>>();
        services.AddTransient<IMyMongoDbReadModelStore<MyTelegram.ReadModel.MongoDB.ReactionReadModel>, MyMongoDbReadModelStore<MyTelegram.ReadModel.MongoDB.ReactionReadModel>>();
        
        services.AddEventHandlers();
    }
}

internal sealed class DocumentReadModelStore : IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.DocumentReadModel>
{
    private readonly IMongoDbContext _dbContext;
    private readonly ILogger _logger;
    private const string CollectionName = "ReadModel-DocumentReadModel";

    public DocumentReadModelStore(
        IMongoDbContext dbContext,
        ILogger<MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.DocumentReadModel>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    private IMongoCollection<MyTelegram.ReadModel.Impl.DocumentReadModel> GetCollection()
    {
        return _dbContext.GetDatabase().GetCollection<MyTelegram.ReadModel.Impl.DocumentReadModel>(CollectionName);
    }

    public IQueryable<MyTelegram.ReadModel.Impl.DocumentReadModel> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public async Task<IReadOnlyCollection<MyTelegram.ReadModel.Impl.DocumentReadModel>> FindAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, bool>> filter,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.DocumentReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        var findOptions = new FindOptions<MyTelegram.ReadModel.Impl.DocumentReadModel>();
        if (skip > 0) findOptions.Skip = skip;
        if (limit > 0) findOptions.Limit = limit;
        
        if (sort != null)
        {
            findOptions.Sort = sort.SortType switch
            {
                SortType.Ascending => Builders<MyTelegram.ReadModel.Impl.DocumentReadModel>.Sort.Ascending(sort.Sort),
                SortType.Descending => Builders<MyTelegram.ReadModel.Impl.DocumentReadModel>.Sort.Descending(sort.Sort),
                _ => null
            };
        }

        var cursor = await GetCollection().FindAsync(filter, findOptions, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<TResult>> FindAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, TResult>> createResult,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.DocumentReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MyTelegram.ReadModel.Impl.DocumentReadModel?> FirstOrDefaultAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var cursor = await GetCollection().FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, TResult>> createResult,
        SortOptions<MyTelegram.ReadModel.Impl.DocumentReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.DocumentReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return GetCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Хранилище для StickerSetReadModel с фиксированным именем коллекции.
/// </summary>
internal sealed class StickerSetReadModelStore : IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StickerSetReadModel>
{
    private readonly IMongoDbContext _dbContext;
    private readonly ILogger _logger;
    private const string CollectionName = "ReadModel-StickerSetReadModel";

    public StickerSetReadModelStore(
        IMongoDbContext dbContext,
        ILogger<MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StickerSetReadModel>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    private IMongoCollection<MyTelegram.ReadModel.Impl.StickerSetReadModel> GetCollection()
    {
        return _dbContext.GetDatabase().GetCollection<MyTelegram.ReadModel.Impl.StickerSetReadModel>(CollectionName);
    }

    public IQueryable<MyTelegram.ReadModel.Impl.StickerSetReadModel> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public async Task<IReadOnlyCollection<MyTelegram.ReadModel.Impl.StickerSetReadModel>> FindAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        var findOptions = new FindOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>();
        if (skip > 0) findOptions.Skip = skip;
        if (limit > 0) findOptions.Limit = limit;
        
        if (sort != null)
        {
            findOptions.Sort = sort.SortType switch
            {
                SortType.Ascending => Builders<MyTelegram.ReadModel.Impl.StickerSetReadModel>.Sort.Ascending(sort.Sort),
                SortType.Descending => Builders<MyTelegram.ReadModel.Impl.StickerSetReadModel>.Sort.Descending(sort.Sort),
                _ => null
            };
        }

        var cursor = await GetCollection().FindAsync(filter, findOptions, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<TResult>> FindAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, TResult>> createResult,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MyTelegram.ReadModel.Impl.StickerSetReadModel?> FirstOrDefaultAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var cursor = await GetCollection().FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, TResult>> createResult,
        SortOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return GetCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Хранилище для BotVerifierReadModel с фиксированным именем коллекции.
/// </summary>
internal sealed class BotVerifierReadModelStore : IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerifierReadModel>
{
    private readonly IMongoDbContext _dbContext;
    private readonly ILogger _logger;
    private const string CollectionName = "ReadModel-BotVerifierReadModel";

    public BotVerifierReadModelStore(
        IMongoDbContext dbContext,
        ILogger<MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerifierReadModel>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    private IMongoCollection<MyTelegram.ReadModel.Impl.BotVerifierReadModel> GetCollection()
    {
        return _dbContext.GetDatabase().GetCollection<MyTelegram.ReadModel.Impl.BotVerifierReadModel>(CollectionName);
    }

    public IQueryable<MyTelegram.ReadModel.Impl.BotVerifierReadModel> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public async Task<IReadOnlyCollection<MyTelegram.ReadModel.Impl.BotVerifierReadModel>> FindAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, bool>> filter,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerifierReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        var findOptions = new FindOptions<MyTelegram.ReadModel.Impl.BotVerifierReadModel>();
        if (skip > 0) findOptions.Skip = skip;
        if (limit > 0) findOptions.Limit = limit;
        
        if (sort != null)
        {
            findOptions.Sort = sort.SortType switch
            {
                SortType.Ascending => Builders<MyTelegram.ReadModel.Impl.BotVerifierReadModel>.Sort.Ascending(sort.Sort),
                SortType.Descending => Builders<MyTelegram.ReadModel.Impl.BotVerifierReadModel>.Sort.Descending(sort.Sort),
                _ => null
            };
        }

        var cursor = await GetCollection().FindAsync(filter, findOptions, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<TResult>> FindAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, TResult>> createResult,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerifierReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MyTelegram.ReadModel.Impl.BotVerifierReadModel?> FirstOrDefaultAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var cursor = await GetCollection().FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, TResult>> createResult,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerifierReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerifierReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return GetCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Хранилище для BotVerificationReadModel с фиксированным именем коллекции.
/// </summary>
internal sealed class BotVerificationReadModelStore : IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerificationReadModel>
{
    private readonly IMongoDbContext _dbContext;
    private readonly ILogger _logger;
    private const string CollectionName = "ReadModel-BotVerificationReadModel";

    public BotVerificationReadModelStore(
        IMongoDbContext dbContext,
        ILogger<MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.BotVerificationReadModel>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    private IMongoCollection<MyTelegram.ReadModel.Impl.BotVerificationReadModel> GetCollection()
    {
        return _dbContext.GetDatabase().GetCollection<MyTelegram.ReadModel.Impl.BotVerificationReadModel>(CollectionName);
    }

    public IQueryable<MyTelegram.ReadModel.Impl.BotVerificationReadModel> GetAll()
    {
        return GetCollection().AsQueryable();
    }

    public async Task<IReadOnlyCollection<MyTelegram.ReadModel.Impl.BotVerificationReadModel>> FindAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, bool>> filter,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerificationReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        var findOptions = new FindOptions<MyTelegram.ReadModel.Impl.BotVerificationReadModel>();
        if (skip > 0) findOptions.Skip = skip;
        if (limit > 0) findOptions.Limit = limit;
        
        if (sort != null)
        {
            findOptions.Sort = sort.SortType switch
            {
                SortType.Ascending => Builders<MyTelegram.ReadModel.Impl.BotVerificationReadModel>.Sort.Ascending(sort.Sort),
                SortType.Descending => Builders<MyTelegram.ReadModel.Impl.BotVerificationReadModel>.Sort.Descending(sort.Sort),
                _ => null
            };
        }

        var cursor = await GetCollection().FindAsync(filter, findOptions, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<TResult>> FindAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, TResult>> createResult,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerificationReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MyTelegram.ReadModel.Impl.BotVerificationReadModel?> FirstOrDefaultAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var cursor = await GetCollection().FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, bool>> filter,
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, TResult>> createResult,
        SortOptions<MyTelegram.ReadModel.Impl.BotVerificationReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(
        Expression<Func<MyTelegram.ReadModel.Impl.BotVerificationReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return GetCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}

/// <summary>
/// Сериализатор, который умеет читать byte[] как из Binary, так и из строки в base64.
/// </summary>
internal sealed class FlexibleByteArraySerializer : MongoDB.Bson.Serialization.Serializers.SerializerBase<byte[]>
{
    public override byte[] Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.GetCurrentBsonType();
        
        return bsonType switch
        {
            BsonType.Binary => context.Reader.ReadBytes(),
            BsonType.String => Convert.FromBase64String(context.Reader.ReadString()),
            BsonType.Null => null!,
            _ => throw new NotSupportedException($"Cannot deserialize byte[] from BsonType {bsonType}")
        };
    }

    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, byte[] value)
    {
        if (value == null)
        {
            context.Writer.WriteNull();
        }
        else
        {
            context.Writer.WriteBytes(value);
        }
    }
}