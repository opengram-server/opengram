using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Domain.Aggregates.Stars;

namespace MyTelegram.Domain.Commands.Stars;

public class CreateStarsAccountCommand(StarsId aggregateId, CommandId commandId, long peerId)
    : Command<StarsAggregate, StarsId, IExecutionResult>(aggregateId, commandId)
{
    public long PeerId { get; } = peerId;
}

public class AddStarsCommand(
    StarsId aggregateId,
    CommandId commandId,
    RequestInfo requestInfo,
    long amount,
    string transactionId,
    string reason)
    : Command<StarsAggregate, StarsId, IExecutionResult>(aggregateId, commandId)
{
    public long Amount { get; } = amount;
    public string TransactionId { get; } = transactionId;
    public string Reason { get; } = reason;
}

public class SpendStarsCommand(
    StarsId aggregateId,
    RequestInfo requestInfo,
    long amount,
    string transactionId,
    string reason)
    : Command<StarsAggregate, StarsId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId}"))
{
    public long Amount { get; } = amount;
    public string TransactionId { get; } = transactionId;
    public string Reason { get; } = reason;
}

public class RefundStarsCommand(
    StarsId aggregateId,
    RequestInfo requestInfo,
    long amount,
    string originalTransactionId,
    string refundTransactionId)
    : Command<StarsAggregate, StarsId, IExecutionResult>(aggregateId, new CommandId($"command-{requestInfo.RequestId}"))
{
    public long Amount { get; } = amount;
    public string OriginalTransactionId { get; } = originalTransactionId;
    public string RefundTransactionId { get; } = refundTransactionId;
}
