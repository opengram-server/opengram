namespace MyTelegram.Domain.Commands.Dialog;

public class ReadMentionCommand(DialogId aggregateId, long ownerUserId, int messageId)
    : Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    public long OwnerUserId { get; } = ownerUserId;

    //public long ToPeerId { get; }
    public int MessageId { get; } = messageId;

    /*long toPeerId,*/
    //ToPeerId = toPeerId;
}