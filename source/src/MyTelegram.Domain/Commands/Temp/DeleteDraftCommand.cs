namespace MyTelegram.Domain.Commands.Temp;

public class DeleteDraftCommand(TempId aggregateId, long ownerPeerId, Peer toPeer) : Command<TempAggregate, TempId, IExecutionResult>(aggregateId)
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}