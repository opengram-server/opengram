namespace MyTelegram.ReadModel.Impl;

using MongoDB.Bson.Serialization.Attributes;

public class StarsReadModel : IStarsReadModel,
    IAmReadModelFor<StarsAggregate, StarsId, StarsAccountCreatedEvent>,
    IAmReadModelFor<StarsAggregate, StarsId, StarsAddedEvent>,
    IAmReadModelFor<StarsAggregate, StarsId, StarsSpentEvent>,
    IAmReadModelFor<StarsAggregate, StarsId, StarsRefundedEvent>
{
    [BsonId]
    public virtual string Id { get; set; } = null!;
    public virtual long PeerId { get; private set; }
    public virtual long Balance { get; private set; }
    public virtual long? Version { get; set; }
    
    public virtual List<StarsTransaction> Transactions { get; private set; } = new();

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarsAggregate, StarsId, StarsAccountCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[StarsReadModel] Applying StarsAccountCreatedEvent. PeerId: {domainEvent.AggregateEvent.PeerId}, Balance: {domainEvent.AggregateEvent.Balance}");
        Id = domainEvent.AggregateIdentity.Value;
        PeerId = domainEvent.AggregateEvent.PeerId;
        Balance = domainEvent.AggregateEvent.Balance;
        Console.WriteLine($"[StarsReadModel] StarsAccountCreatedEvent applied. Id: {Id}, PeerId: {PeerId}, Balance: {Balance}");
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarsAggregate, StarsId, StarsAddedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        Balance = domainEvent.AggregateEvent.NewBalance;
        
        Transactions.Add(new StarsTransaction
        {
            Id = domainEvent.AggregateEvent.TransactionId,
            Amount = domainEvent.AggregateEvent.Amount,
            Type = StarsTransactionType.TopUp,
            Reason = domainEvent.AggregateEvent.Reason,
            Date = domainEvent.Timestamp.DateTime
        });
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarsAggregate, StarsId, StarsSpentEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[StarsReadModel] Applying StarsSpentEvent. PeerId: {PeerId}, OldBalance: {Balance}, NewBalance: {domainEvent.AggregateEvent.NewBalance}, Amount: {domainEvent.AggregateEvent.Amount}");
        Id = domainEvent.AggregateIdentity.Value;
        Balance = domainEvent.AggregateEvent.NewBalance;
        
        Transactions.Add(new StarsTransaction
        {
            Id = domainEvent.AggregateEvent.TransactionId,
            Amount = -domainEvent.AggregateEvent.Amount, // отрицательное значение при списании
            Type = StarsTransactionType.Purchase,
            Reason = domainEvent.AggregateEvent.Reason,
            Date = domainEvent.Timestamp.DateTime
        });
        Console.WriteLine($"[StarsReadModel] StarsSpentEvent applied. New balance: {Balance}, Transaction added to history");
        
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<StarsAggregate, StarsId, StarsRefundedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        Balance = domainEvent.AggregateEvent.NewBalance;
        
        Transactions.Add(new StarsTransaction
        {
            Id = domainEvent.AggregateEvent.RefundTransactionId,
            Amount = domainEvent.AggregateEvent.Amount,
            Type = StarsTransactionType.Refund,
            Reason = $"Refund for {domainEvent.AggregateEvent.OriginalTransactionId}",
            Date = domainEvent.Timestamp.DateTime
        });
        
        return Task.CompletedTask;
    }
}
