using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

public class UserDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IEventBus eventBus,
    ILogger<UserDomainEventHandler> logger,
    IPhotoAppService photoAppService,
    ILayeredService<IPhotoConverter> photoLayeredConverter,
    ILayeredService<IAuthorizationConverter> layeredAuthorizationService,
    IUserConverterService userConverterService,
    IReadModelCacheHelper<IUserReadModel> userReadModelCacheHelper,
    IReadModelCacheHelper<IUserFullReadModel> userFullReadModelCacheHelper)
    : DomainEventHandlerBase(objectMessageSender,
            commandBus,
            idGenerator,
            ackCacheService),
        ISubscribeSynchronousTo<UserAggregate, UserId, UserCreatedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserProfileUpdatedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserNameUpdatedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserProfilePhotoChangedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserProfilePhotoUploadedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, UserEmojiStatusUpdatedEvent>,
        ISubscribeSynchronousTo<UserAggregate, UserId, BusinessIntroUpdatedEvent>
{
    public Task HandleAsync(IDomainEvent<UserAggregate, UserId, BusinessIntroUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.UserId;
        // Сбрасываем кэш, чтобы следующий запрос получил свежие данные из базы
        userReadModelCacheHelper.Remove(userId);
        userFullReadModelCacheHelper.Remove(userId);
        
        return Task.CompletedTask;
    }
    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "User created successfully, userId: {UserId}  phoneNumber: {PhoneNumber} firstName: {FirstName} lastName: {LastName}",
            domainEvent.AggregateEvent.UserId,
            domainEvent.AggregateEvent.PhoneNumber,
            domainEvent.AggregateEvent.FirstName,
            domainEvent.AggregateEvent.LastName
        );

        var userId = domainEvent.AggregateEvent.UserId;

        await eventBus.PublishAsync(new UserSignUpSuccessIntegrationEvent(
            domainEvent.AggregateEvent.RequestInfo.AuthKeyId,
            domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId,
            userId));
        var user = await userConverterService.GetUserAsync(domainEvent.AggregateEvent.RequestInfo, userId, layer: domainEvent.AggregateEvent.RequestInfo.Layer);
        user.Self = true;
        var r = layeredAuthorizationService.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer)
            .CreateAuthorization(user);
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo,
            r,
            domainEvent.AggregateEvent.UserId);
    }

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserNameUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.RequestInfo.UserId;
        if (userId == 0)
        {
            return;
        }
        var user = await userConverterService.GetUserAsync(domainEvent.AggregateEvent.RequestInfo, userId, layer: domainEvent.AggregateEvent.RequestInfo.Layer);

        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, user);
    }

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserProfilePhotoChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.RequestInfo.UserId;
        var user = await userConverterService.GetUserAsync(domainEvent.AggregateEvent.RequestInfo, userId, layer: domainEvent.AggregateEvent.RequestInfo.Layer);
        var photoReadModel = await photoAppService.GetAsync(domainEvent.AggregateEvent.PhotoId);

        var photo = new MyTelegram.Schema.Photos.TPhoto
        {
            Photo = photoLayeredConverter.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer).ToPhoto(photoReadModel),
            Users = [user]
        };

        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, photo);
    }

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserProfilePhotoUploadedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.RequestInfo.UserId;
        var user = await userConverterService.GetUserAsync(domainEvent.AggregateEvent.RequestInfo, userId, layer: domainEvent.AggregateEvent.RequestInfo.Layer);
        var photoReadModel = await photoAppService.GetAsync(domainEvent.AggregateEvent.PhotoId);

        var photo = new MyTelegram.Schema.Photos.TPhoto
        {
            Photo = photoLayeredConverter.GetConverter(domainEvent.AggregateEvent.RequestInfo.Layer).ToPhoto(photoReadModel),
            Users = [user]
        };

        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, photo);
    }

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserProfileUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.RequestInfo.UserId;
        var user = await userConverterService.GetUserAsync(domainEvent.AggregateEvent.RequestInfo, userId, layer: domainEvent.AggregateEvent.RequestInfo.Layer);
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, user, domainEvent.AggregateEvent.UserId);
    }

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserEmojiStatusUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var userId = domainEvent.AggregateEvent.UserId;

        // Формируем объект emoji-статуса
        IEmojiStatus emojiStatus;
        if (domainEvent.AggregateEvent.EmojiStatusDocumentId.HasValue)
        {
            emojiStatus = new TEmojiStatus
            {
                DocumentId = domainEvent.AggregateEvent.EmojiStatusDocumentId.Value,
                Until = domainEvent.AggregateEvent.EmojiStatusValidUntil
            };
        }
        else
        {
            emojiStatus = new TEmojiStatusEmpty();
        }

        // Формируем update для пользователя
        var update = new TUpdateUserEmojiStatus
        {
            UserId = userId,
            EmojiStatus = emojiStatus
        };

        // Загружаем пользователя, чтобы включить его в update
        var user = await userConverterService.GetUserAsync(
            domainEvent.AggregateEvent.RequestInfo,
            userId,
            layer: domainEvent.AggregateEvent.RequestInfo.Layer);

        // Также отправляем updateRecentEmojiStatuses, чтобы клиент перезагрузил эмодзи
        var recentEmojiUpdate = new TUpdateRecentEmojiStatuses();

        // Оборачиваем апдейты в TUpdates вместе с пользователем
        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update, recentEmojiUpdate), // Включаем оба апдейта
            Users = new TVector<IUser>(user), // Добавляем пользователя, чтобы клиент увидел emoji-статус
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp()
        };

        // Отправляем update клиенту, инициировавшему запрос
        await SendRpcMessageToClientAsync(domainEvent.AggregateEvent.RequestInfo, new TBoolTrue());

        // Отправляем update всем контактам, которые могут видеть emoji-статус этого пользователя
        await PushUpdatesToPeerAsync(new Peer(PeerType.User, userId), updates, 
            excludeAuthKeyId: domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId);
    }
}