using EventFlow.ValueObjects;
using MyTelegram.Domain.Aggregates.StarGift;
using MyTelegram.Domain.Aggregates.Stars;
using MyTelegram.Domain.Commands.StarGift;
using MyTelegram.Domain.Commands.Stars;
using MyTelegram.Domain.Events.StarGift;
using MyTelegram.Domain.Events.Stars;

namespace MyTelegram.Domain.Sagas;

public class StarGiftSagaStartedEvent(string giftAggregateId, RequestInfo requestInfo, long fromUserId, long toUserId, long totalStars)
    : AggregateEvent<StarGiftSaga, StarGiftSagaId>
{
    public string GiftAggregateId { get; } = giftAggregateId;
    public RequestInfo RequestInfo { get; } = requestInfo;
    public long FromUserId { get; } = fromUserId;
    public long ToUserId { get; } = toUserId;
    public long TotalStars { get; } = totalStars;
}

public class StarGiftSaga : MyInMemoryAggregateSaga<StarGiftSaga, StarGiftSagaId, StarGiftSagaLocator>,
    ISagaIsStartedBy<StarGiftAggregate, StarGiftId, StarGiftInitiatedEvent>,
    ISagaHandles<StarsAggregate, StarsId, StarsSpentEvent>
{
    private readonly StarGiftSagaState _state = new();

    public StarGiftSaga(StarGiftSagaId id, IEventStore eventStore) : base(id, eventStore)
    {
        Console.WriteLine($"[StarGiftSaga] Constructor called with SagaId: {id.Value}");
        Register(_state);
        Console.WriteLine($"[StarGiftSaga] State registered. Current GiftAggregateId: {_state.GiftAggregateId?.Value ?? "NULL"}");
    }

    public Task HandleAsync(IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftInitiatedEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        var totalStars = evt.Stars;

        // Добавляем UpgradeStars только если CanUpgrade = false.
        // CanUpgrade = false означает, что подарок отправляется уже с включённым апгрейдом,
        // CanUpgrade = true означает, что подарок можно улучшить позже (стоимость сейчас не включается)
        if (!evt.CanUpgrade && evt.UpgradeStars.HasValue)
        {
            totalStars += evt.UpgradeStars.Value;
            Console.WriteLine($"[StarGiftSaga] Including upgrade cost: {evt.UpgradeStars.Value} stars");
        }

        Console.WriteLine($"[StarGiftSaga] StarGiftInitiatedEvent received. AggregateId: {evt.AggregateIdValue}, FromUser: {evt.FromUserId}, ToUser: {evt.ToUserId}, Stars: {totalStars}, CanUpgrade: {evt.CanUpgrade}");
        Console.WriteLine($"[StarGiftSaga] Current state - GiftAggregateId: {_state.GiftAggregateId?.Value ?? "NULL"}");

        var command = new SpendStarsCommand(
            StarsId.Create(evt.FromUserId),
            domainEvent.AggregateEvent.RequestInfo,
            totalStars,
            evt.AggregateIdValue, // TransactionId = StarGiftId
            $"Gift: {evt.GiftId} to {evt.ToUserId}" // Reason
        );

        Console.WriteLine($"[StarGiftSaga] Publishing SpendStarsCommand. TransactionId: {evt.AggregateIdValue}, Amount: {totalStars}");

        // Эмитим событие саги, чтобы её состояние сохранилось
        Emit(new StarGiftSagaStartedEvent(evt.AggregateIdValue, evt.RequestInfo, evt.FromUserId, evt.ToUserId, totalStars));

        Publish(command);
        Console.WriteLine($"[StarGiftSaga] Command published, saga should be persisted now");
        return Task.CompletedTask;
    }

    public Task HandleAsync(IDomainEvent<StarsAggregate, StarsId, StarsSpentEvent> domainEvent, ISagaContext sagaContext, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        Console.WriteLine($"[StarGiftSaga] StarsSpentEvent received. TransactionId: {evt.TransactionId}, Amount: {evt.Amount}, GiftAggregateId: {_state.GiftAggregateId?.Value ?? "NULL"}");

        if (_state.GiftAggregateId != null)
        {
            Console.WriteLine($"[StarGiftSaga] Publishing CompleteStarGiftCommand for GiftId: {_state.GiftAggregateId.Value}");

            // Создаём новый RequestInfo с новым RequestId, чтобы не получить дублирующийся CommandId
            var newRequestInfo = new RequestInfo(
                _state.RequestInfo.ReqMsgId,
                _state.RequestInfo.UserId,
                _state.RequestInfo.AccessHashKeyId,
                _state.RequestInfo.AuthKeyId,
                _state.RequestInfo.PermAuthKeyId,
                Guid.NewGuid(), // Новый уникальный RequestId для CompleteStarGiftCommand
                _state.RequestInfo.Layer,
                _state.RequestInfo.Date,
                _state.RequestInfo.DeviceType,
                _state.RequestInfo.AddRequestIdToCache,
                _state.RequestInfo.IsSubRequest
            );
            
            var command = new CompleteStarGiftCommand(
                _state.GiftAggregateId,
                newRequestInfo
            );
            Publish(command);
            return CompleteAsync(cancellationToken);
        }

        Console.WriteLine($"[StarGiftSaga] GiftAggregateId is NULL! Cannot complete gift.");
        return Task.CompletedTask;
    }
}

public class StarGiftSagaLocator : ISagaLocator
{
    public Task<ISagaId> LocateSagaAsync(IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        switch (domainEvent)
        {
            case IDomainEvent<StarGiftAggregate, StarGiftId, StarGiftInitiatedEvent> initiated:
                // AggregateIdentity.Value уже содержит полный идентификатор (например, "stargift-guid")
                var sagaIdInitiated = initiated.AggregateIdentity.Value;
                Console.WriteLine($"[StarGiftSagaLocator] Locating saga for StarGiftInitiatedEvent: {sagaIdInitiated}");
                return Task.FromResult<ISagaId>(new StarGiftSagaId(sagaIdInitiated));

            case IDomainEvent<StarsAggregate, StarsId, StarsSpentEvent> spent:
                // TransactionId уже содержит полный идентификатор (например, "stargift-guid")
                var sagaIdSpent = spent.AggregateEvent.TransactionId;
                Console.WriteLine($"[StarGiftSagaLocator] Locating saga for StarsSpentEvent: {sagaIdSpent} (TransactionId: {spent.AggregateEvent.TransactionId})");
                return Task.FromResult<ISagaId>(new StarGiftSagaId(sagaIdSpent));
        }

        Console.WriteLine($"[StarGiftSagaLocator] Unknown event type: {domainEvent.GetType().Name}");
        return Task.FromResult<ISagaId>(null!);
    }
}

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<StarGiftSagaId>))]
public class StarGiftSagaId(string value) : SingleValueObject<string>(value), ISagaId;

public class StarGiftSagaState : AggregateState<StarGiftSaga, StarGiftSagaId, StarGiftSagaState>,
    IApply<StarGiftInitiatedEvent>,
    IApply<StarGiftSagaStartedEvent>
{
    public StarGiftId? GiftAggregateId { get; private set; }
    public RequestInfo RequestInfo { get; private set; }

    public void Apply(StarGiftInitiatedEvent aggregateEvent)
    {
        Console.WriteLine($"[StarGiftSagaState] Applying StarGiftInitiatedEvent. AggregateId: {aggregateEvent.AggregateIdValue}");
        GiftAggregateId = StarGiftId.Create(aggregateEvent.AggregateIdValue);
        RequestInfo = aggregateEvent.RequestInfo;
        Console.WriteLine($"[StarGiftSagaState] State updated. GiftAggregateId: {GiftAggregateId.Value}");
    }

    public void Apply(StarGiftSagaStartedEvent aggregateEvent)
    {
        Console.WriteLine($"[StarGiftSagaState] Applying StarGiftSagaStartedEvent. GiftAggregateId: {aggregateEvent.GiftAggregateId}");
        GiftAggregateId = StarGiftId.Create(aggregateEvent.GiftAggregateId);
        RequestInfo = aggregateEvent.RequestInfo;
        Console.WriteLine($"[StarGiftSagaState] GiftAggregateId set to: {GiftAggregateId.Value}, RequestInfo: {(RequestInfo != null ? "SET" : "NULL")}");
    }
}
