using MyTelegram.Messenger.DomainEventHandlers;
using MyTelegram.Domain.Commands.Privacy;
using MyTelegram;

namespace MyTelegram.Messenger.CommandServer.DomainEventHandlers;

public class UserDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    IMessageAppService messageAppService,
    IOptionsMonitor<MyTelegramMessengerServerOptions> options,
    IRandomHelper randomHelper)
    : DomainEventHandlerBase(objectMessageSender, commandBus, idGenerator, ackCacheService),
        ISubscribeSynchronousTo<UserAggregate, UserId, UserCreatedEvent>
{
    private readonly ICommandBus _commandBus = commandBus;

    public async Task HandleAsync(IDomainEvent<UserAggregate, UserId, UserCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (options.CurrentValue.SetPremiumToTrueAfterUserCreated)
        {
            var command = new UpdateUserPremiumStatusCommand(domainEvent.AggregateIdentity, true);
            await _commandBus.PublishAsync(command, default);
        }

        // Set default privacy rules for new users
        // By default, hide phone number from everyone
        var privacyValueDataList = new List<PrivacyValueData>
        {
            new PrivacyValueData(PrivacyValueType.DisallowAll)
        };
        var setPrivacyCommand = new SetPrivacyCommand(domainEvent.AggregateIdentity, domainEvent.AggregateEvent.RequestInfo, PrivacyType.PhoneNumber, privacyValueDataList);
        await _commandBus.PublishAsync(setPrivacyCommand, CancellationToken.None);

        if (!options.CurrentValue.SendWelcomeMessageAfterUserSignIn)
        {
            return;
        }

        if (!domainEvent.AggregateEvent.Bot)
        {
            var welcomeMessage = "Welcome to use MyTelegram!";
            var sendMessageInput = new SendMessageInput(
                RequestInfo.Empty with
                {
                    UserId = MyTelegramConsts.OfficialUserId,
                    AuthKeyId = domainEvent.AggregateEvent.RequestInfo.AuthKeyId,
                    PermAuthKeyId = domainEvent.AggregateEvent.RequestInfo.PermAuthKeyId,
                    Date = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                    RequestId = Guid.NewGuid(),
                    DeviceType = DeviceType.Desktop
                },
                MyTelegramConsts.OfficialUserId,
                new Peer(PeerType.User, domainEvent.AggregateEvent.UserId/*, domainEvent.AggregateEvent.AccessHash*/),
                welcomeMessage,
                randomHelper.NextInt64());

            await messageAppService.SendMessageAsync([sendMessageInput]);
        }
    }
}