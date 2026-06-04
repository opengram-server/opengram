namespace MyTelegram.Domain.Commands.Dialog;

public class OutboxMessageHasReadCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    int maxMessageId,
    long ownerPeerId,
    Peer toPeer)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public int MaxMessageId { get; } = maxMessageId;
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}