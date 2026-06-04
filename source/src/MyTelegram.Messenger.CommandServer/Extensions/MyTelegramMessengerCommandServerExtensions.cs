using EventFlow.Core.Caching;
using EventFlow.MongoDB.Extensions;
using MyTelegram.EventBus.Extensions;
using MyTelegram.EventFlow.MongoDB.Extensions;
using MyTelegram.EventFlow.MongoDB.ReadStores;
using MyTelegram.EventFlow.ReadStores;
using MyTelegram.Messenger.CommandServer.EventHandlers;
using MyTelegram.Messenger.NativeAot;
using MyTelegram.QueryHandlers.MongoDB;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.Services.Extensions;
using MyTelegram.Domain.Shared.Events;
using MongoDB.Driver;
using MyTelegram.EventFlow.MongoDB;

namespace MyTelegram.Messenger.CommandServer.Extensions;

public static class MyTelegramMessengerCommandServerExtensions
{
    public static void AddEventHandlers(this IServiceCollection services)
    {
        services.AddSubscription<MessengerCommandDataReceivedEvent, MessengerEventHandler>();
        services.AddSubscription<NewDeviceCreatedEvent, MessengerEventHandler>();
        services.AddSubscription<BindUserIdToAuthKeyIntegrationEvent, MessengerEventHandler>();
        services.AddSubscription<AuthKeyUnRegisteredIntegrationEvent, MessengerEventHandler>();

        services.AddSubscription<NewPtsMessageHasSentEvent, PtsEventHandler>();
        services.AddSubscription<RpcMessageHasSentEvent, PtsEventHandler>();
        services.AddSubscription<AcksDataReceivedEvent, PtsEventHandler>();

        // События Bot API
        services.AddSubscription<BotMessageEvent, BotMessageEventHandler>();
        services.AddSubscription<BotGiftEvent, BotGiftEventHandler>();

        // События служебных уведомлений
        services.AddSubscription<ServiceNotificationEvent, ServiceNotificationEventHandler>();

        // События подарков за звёзды
        services.AddSubscription<StarGiftUpgradedIntegrationEvent, StarGiftUpgradedIntegrationEventHandler>();
        services.AddSubscription<StarGiftListedForResaleIntegrationEvent, UpdateCatalogResaleCountHandler>();
        services.AddSubscription<StarGiftRemovedFromResaleIntegrationEvent, UpdateCatalogResaleCountHandler>();
    }

    public static void AddMyTelegramMessengerCommandServer(this IServiceCollection services,
        Action<IEventFlowOptions>? configure = null)
    {
        services.RegisterServices();
        services.AddMyTelegramMessengerServices();

        services.AddEventFlow(options =>
        {
            options.AddDefaults(typeof(MyTelegramMessengerServerExtensions).Assembly);
            options.AddDefaults(typeof(MyTelegram.Domain.Specs).Assembly);
            
            // Большинство CommandHandler-ов уже регистрируются через AddDefaults выше.
            // Здесь регистрируем только те, что лежат вне сборки Domain или требуют особой настройки.
            // Дублирующие регистрации убраны, иначе EventFlow ругается на "Too many command handlers".

            // Саги, отвечающие за работу с сообщениями
            options.AddSagas(
                typeof(MyTelegram.Domain.Sagas.SendMessageSaga),
                typeof(MyTelegram.Domain.Sagas.ForwardMessageSaga),
                typeof(MyTelegram.Domain.Sagas.EditMessageSaga),
                typeof(MyTelegram.Domain.Sagas.ReadHistorySaga),
                typeof(MyTelegram.Domain.Sagas.ReadChannelHistorySaga),
                typeof(MyTelegram.Domain.Sagas.DeleteMessagesSaga4),
                typeof(MyTelegram.Domain.Sagas.DeleteReplyMessagesSaga)
            );
            
            // Саги управления каналами и пользователями
            options.AddSagas(
                typeof(MyTelegram.Domain.Sagas.CreateChannelSaga),
                typeof(MyTelegram.Domain.Sagas.InviteToChannelSaga),
                typeof(MyTelegram.Domain.Sagas.JoinChannelSaga),
                typeof(MyTelegram.Domain.Sagas.LeaveChannelSaga),
                typeof(MyTelegram.Domain.Sagas.ApproveJoinChannelSaga),
                typeof(MyTelegram.Domain.Sagas.EditAdminSaga),
                typeof(MyTelegram.Domain.Sagas.EditBannedSaga)
            );
            
            // Прочие саги
            options.AddSagas(
                typeof(MyTelegram.Domain.Sagas.CreateUserSaga),
                typeof(MyTelegram.Domain.Sagas.SignInSaga),
                typeof(MyTelegram.Domain.Sagas.UserSignUpSaga),
                typeof(MyTelegram.Domain.Sagas.ImportContactsSaga),
                typeof(MyTelegram.Domain.Sagas.UpdateUserNameSaga),
                typeof(MyTelegram.Domain.Sagas.UploadProfilePhotoSaga),
                typeof(MyTelegram.Domain.Sagas.UpdateContactProfilePhotoSaga),
                typeof(MyTelegram.Domain.Sagas.ClearHistorySaga),
                typeof(MyTelegram.Domain.Sagas.EditChannelPhotoSaga),
                typeof(MyTelegram.Domain.Sagas.EditChannelTitleSaga),
                typeof(MyTelegram.Domain.Sagas.EditExportedChatInviteSaga),
                typeof(MyTelegram.Domain.Sagas.EditPeerFoldersSaga),
                typeof(MyTelegram.Domain.Sagas.ImportChatInviteSaga),
                typeof(MyTelegram.Domain.Sagas.PinForwardedChannelMessageSaga),
                typeof(MyTelegram.Domain.Sagas.SetDiscussionGroupSaga),
                typeof(MyTelegram.Domain.Sagas.SetHistoryTTLSaga),
                typeof(MyTelegram.Domain.Sagas.UnpinAllMessagesSaga),
                typeof(MyTelegram.Domain.Sagas.UpdateMessagePinnedSaga),
                typeof(MyTelegram.Domain.Sagas.UpdatePinnedMessageSaga),
                typeof(MyTelegram.Domain.Sagas.UpdatePinnedMessageSaga),
                typeof(MyTelegram.Domain.Sagas.VoteSaga),
                typeof(MyTelegram.Domain.Sagas.StarGiftSaga)
            );
            
            // Подписчики на события
            options.AddSubscribers(typeof(StickerSetInstalledEventHandler));
            options.AddSubscribers(typeof(ScheduledMessageCreatedEventHandler));
            options.AddSubscribers(typeof(MyTelegram.Messenger.CommandServer.DomainEventHandlers.StarGiftDomainEventHandler));
            
            options.Configure(c => { c.IsAsynchronousSubscribersEnabled = true; });

            options.UseMongoDbEventStore();
            options.UseMongoDbSnapshotStore();

            options.AddMyTelegramMongoDbReadModel();
            options.AddMongoDbQueryHandlers();

            options.AddSystemTextJson(jsonSerializerOptions =>
            {
                jsonSerializerOptions.AddSingleValueObjects(
                    new EventFlow.SystemTextJsonSingleValueObjectConverter<CacheKey>());
                jsonSerializerOptions.TypeInfoResolverChain.Add(MyMessengerJsonContext.Default);
            });
            configure?.Invoke(options);
        });
		services.AddEventStoreMongoDbContext();
        services.AddReadModelMongoDbContext();
        
        // PrivacyReadModel — только для чтения
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.PrivacyReadModel>, MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.PrivacyReadModel>>();
        // StickerSetReadModel с собственным именем коллекции (как на Query Server)
        services.AddTransient<IQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StickerSetReadModel>>(sp =>
        {
            var dbContext = sp.GetRequiredService<IMongoDbContext>();
            var logger = sp.GetRequiredService<ILogger<MongoDbQueryOnlyReadModelStore<MyTelegram.ReadModel.Impl.StickerSetReadModel>>>();
            return new StickerSetReadModelStore(dbContext, logger);
        });
        
        // Локаторы read-моделей должны подхватываться автоматически, но DI их не находит,
        // поэтому регистрируем их явно.
        services.AddTransient<MyTelegram.ReadModel.ReadModelLocators.IUserReadModelLocator, MyTelegram.ReadModel.ReadModelLocators.UserReadModelLocator>();
        services.AddTransient<MyTelegram.ReadModel.ReadModelLocators.IMessageIdLocator, MyTelegram.ReadModel.ReadModelLocators.MessageIdLocator>();
        services.AddTransient<MyTelegram.ReadModel.ReadModelLocators.IDialogReadModelLocator, MyTelegram.ReadModel.ReadModelLocators.DialogReadModelLocator>();
        services.AddTransient<MyTelegram.ReadModel.ReadModelLocators.IChannelReadModelLocator, MyTelegram.ReadModel.ReadModelLocators.ChannelReadModelLocator>();
        
        services.AddTransient<Handlers.LatestLayer.Impl.Payments.GetStarGiftHandler>();
        services.AddTransient<Handlers.LatestLayer.Interfaces.Payments.IGetStarGiftHandler, Handlers.LatestLayer.Impl.Payments.GetStarGiftHandler>();
        
        // Обработчики SaveStarGift, TransferStarGift, UpgradeStarGift регистрируются автоматически через рефлексию
        
        services.AddEventHandlers();
    }
}

/// <summary>
/// Хранилище для StickerSetReadModel с фиксированным именем коллекции (скопировано с Query Server).
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
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
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
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, TResult>> createResult,
        int skip = 0,
        int limit = 0,
        SortOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<MyTelegram.ReadModel.Impl.StickerSetReadModel?> FirstOrDefaultAsync(
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        var cursor = await GetCollection().FindAsync(filter, cancellationToken: cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public Task<TResult?> FirstOrDefaultAsync<TResult>(
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, TResult>> createResult,
        SortOptions<MyTelegram.ReadModel.Impl.StickerSetReadModel>? sort = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<long> CountAsync(
        System.Linq.Expressions.Expression<Func<MyTelegram.ReadModel.Impl.StickerSetReadModel, bool>> filter,
        CancellationToken cancellationToken = default)
    {
        return GetCollection().CountDocumentsAsync(filter, cancellationToken: cancellationToken);
    }
}
