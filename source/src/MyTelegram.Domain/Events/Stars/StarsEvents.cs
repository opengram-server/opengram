using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Domain.Aggregates.Stars;

namespace MyTelegram.Domain.Events.Stars;

public class StarsAccountCreatedEvent(long peerId, long balance)
    : RequestAggregateEvent2<StarsAggregate, StarsId>(RequestInfo.Empty)
{
    public long PeerId { get; } = peerId;
    public long Balance { get; } = balance;
}

public class StarsAddedEvent(
    long peerId,
    long amount,
    string transactionId,
    string reason,
    long newBalance)
    : RequestAggregateEvent2<StarsAggregate, StarsId>(RequestInfo.Empty)
{
    public long PeerId { get; } = peerId;
    public long Amount { get; } = amount;
    public string TransactionId { get; } = transactionId;
    public string Reason { get; } = reason;
    public long NewBalance { get; } = newBalance;
}

public class StarsSpentEvent(
    long peerId,
    long amount,
    string transactionId,
    string reason,
    long newBalance)
    : RequestAggregateEvent2<StarsAggregate, StarsId>(RequestInfo.Empty)
{
    public long PeerId { get; } = peerId;
    public long Amount { get; } = amount;
    public string TransactionId { get; } = transactionId;
    public string Reason { get; } = reason;
    public long NewBalance { get; } = newBalance;
}

public class StarsRefundedEvent(
    long peerId,
    long amount,
    string originalTransactionId,
    string refundTransactionId,
    long newBalance)
    : RequestAggregateEvent2<StarsAggregate, StarsId>(RequestInfo.Empty)
{
    public long PeerId { get; } = peerId;
    public long Amount { get; } = amount;
    public string OriginalTransactionId { get; } = originalTransactionId;
    public string RefundTransactionId { get; } = refundTransactionId;
    public long NewBalance { get; } = newBalance;
}
