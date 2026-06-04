using MyTelegram.Domain.Events.Stars;
using MyTelegram.Domain.Shared.Stars;
using EventFlow.Exceptions;
using MyTelegram.Domain;

namespace MyTelegram.Domain.Aggregates.Stars;

public class StarsSnapshot : ISnapshot
{
    public long PeerId { get; set; }
    public long Balance { get; set; }
}

[JsonConverter(typeof(SystemTextJsonSingleValueObjectConverter<StarsId>))]
public class StarsId : Identity<StarsId>
{
    public StarsId(string value) : base(value) { }
    
    public static StarsId Create(long peerId)
    {
        return NewDeterministic(GuidFactories.Deterministic.Namespaces.Commands, $"stars_{peerId}");
    }
}

public class StarsAggregate : MyInMemorySnapshotAggregateRoot<StarsAggregate, StarsId, StarsSnapshot>
{
    private long _peerId;
    private long _balance;

    public StarsAggregate(StarsId id) : base(id, SnapshotEveryFewVersionsStrategy.Default)
    {
    }

    public void Create(long peerId)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StarsAccountCreatedEvent(peerId, 0));
    }

    public void AddStars(long amount, string transactionId, string reason)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Emit(new StarsAddedEvent(_peerId, amount, transactionId, reason, _balance + amount));
    }

    public void SpendStars(long amount, string transactionId, string reason)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        if (_balance < amount)
        {
            // Недостаточно средств — возвращаем соответствующую RPC-ошибку
            RpcErrors.RpcErrors400.BalanceTooLow.ThrowRpcError();
        }

        Console.WriteLine($"[StarsAggregate] Spending {amount} stars. TransactionId: {transactionId}, Balance: {_balance} -> {_balance - amount}");
        Emit(new StarsSpentEvent(_peerId, amount, transactionId, reason, _balance - amount));
        Console.WriteLine($"[StarsAggregate] StarsSpentEvent emitted with TransactionId: {transactionId}");
    }

    public void RefundStars(long amount, string originalTransactionId, string refundTransactionId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        if (amount <= 0)
        {
            throw new ArgumentException("Amount must be positive", nameof(amount));
        }

        Emit(new StarsRefundedEvent(_peerId, amount, originalTransactionId, refundTransactionId, _balance + amount));
    }

    protected override Task<StarsSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new StarsSnapshot
        {
            PeerId = _peerId,
            Balance = _balance
        });
    }

    protected override Task LoadSnapshotAsync(StarsSnapshot snapshot, ISnapshotMetadata metadata, CancellationToken cancellationToken)
    {
        _peerId = snapshot.PeerId;
        _balance = snapshot.Balance;
        return Task.CompletedTask;
    }

    public void Apply(StarsAccountCreatedEvent aggregateEvent)
    {
        _peerId = aggregateEvent.PeerId;
        _balance = aggregateEvent.Balance;
    }

    public void Apply(StarsAddedEvent aggregateEvent)
    {
        _balance = aggregateEvent.NewBalance;
    }

    public void Apply(StarsSpentEvent aggregateEvent)
    {
        _balance = aggregateEvent.NewBalance;
    }

    public void Apply(StarsRefundedEvent aggregateEvent)
    {
        _balance = aggregateEvent.NewBalance;
    }
}
