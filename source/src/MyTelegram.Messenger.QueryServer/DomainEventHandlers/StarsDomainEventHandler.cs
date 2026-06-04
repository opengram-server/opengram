using MyTelegram.Domain.Aggregates.Stars;
using MyTelegram.Domain.Events.Stars;
using MyTelegram.Messenger.Services.Interfaces;

namespace MyTelegram.Messenger.QueryServer.DomainEventHandlers;

public class StarsDomainEventHandler(
    IObjectMessageSender objectMessageSender,
    ICommandBus commandBus,
    IIdGenerator idGenerator,
    IAckCacheService ackCacheService,
    ILogger<StarsDomainEventHandler> logger)
    : DomainEventHandlerBase(objectMessageSender,
            commandBus,
            idGenerator,
            ackCacheService),
        ISubscribeSynchronousTo<StarsAggregate, StarsId, StarsSpentEvent>,
        ISubscribeSynchronousTo<StarsAggregate, StarsId, StarsAddedEvent>,
        ISubscribeSynchronousTo<StarsAggregate, StarsId, StarsRefundedEvent>
{
    public Task HandleAsync(IDomainEvent<StarsAggregate, StarsId, StarsSpentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Console.WriteLine($"[StarsDomainEventHandler] StarsSpentEvent received! PeerId={evt.PeerId} Amount={evt.Amount} NewBalance={evt.NewBalance} TransactionId={evt.TransactionId}");
        logger.LogInformation("StarsSpentEvent: PeerId={PeerId} Amount={Amount} NewBalance={NewBalance} TransactionId={TransactionId}",
            evt.PeerId, evt.Amount, evt.NewBalance, evt.TransactionId);

        return SendUpdateAsync(evt.PeerId, evt.NewBalance);
    }

    public Task HandleAsync(IDomainEvent<StarsAggregate, StarsId, StarsAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        logger.LogInformation("StarsAddedEvent: PeerId={PeerId} Amount={Amount} NewBalance={NewBalance} TransactionId={TransactionId}",
            evt.PeerId, evt.Amount, evt.NewBalance, evt.TransactionId);

        return SendUpdateAsync(evt.PeerId, evt.NewBalance);
    }

    public Task HandleAsync(IDomainEvent<StarsAggregate, StarsId, StarsRefundedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        logger.LogInformation("StarsRefundedEvent: PeerId={PeerId} Amount={Amount} NewBalance={NewBalance} TransactionId={TransactionId}",
            evt.PeerId, evt.Amount, evt.NewBalance, evt.RefundTransactionId);

        return SendUpdateAsync(evt.PeerId, evt.NewBalance);
    }

    private async Task SendUpdateAsync(long userId, long newBalance)
    {
        Console.WriteLine($"[StarsDomainEventHandler] Creating updateStarsBalance for UserId={userId}, NewBalance={newBalance}");

        var update = new TUpdateStarsBalance
        {
            Balance = new TStarsAmount
            {
                Amount = newBalance,
                Nanos = 0 // Поле nanos пока не используется, оставляем 0
            }
        };

        var updates = new TUpdates
        {
            Updates = new TVector<IUpdate>(update),
            Users = new TVector<IUser>(),
            Chats = new TVector<IChat>(),
            Date = DateTime.UtcNow.ToTimestamp(),
            Seq = 0
        };

        Console.WriteLine($"[StarsDomainEventHandler] Sending updateStarsBalance to UserId={userId}");
        await PushUpdatesToPeerAsync(new Peer(PeerType.User, userId), updates);
        Console.WriteLine($"[StarsDomainEventHandler] updateStarsBalance sent to UserId={userId}");
    }
}
