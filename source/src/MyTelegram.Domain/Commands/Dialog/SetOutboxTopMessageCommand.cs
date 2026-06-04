namespace MyTelegram.Domain.Commands.Dialog;

public class SetOutboxTopMessageCommand(
    DialogId aggregateId,
    int messageId,
    long ownerPeerId,
    Peer toPeer,
    bool clearDraft)
    : /*Request*/Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId)
{
    //long reqMsgId,
    //RequestInfo requestInfo,
    //int pts,
    //MessageBoxId = messageBoxId;
    //Pts = pts; 

    public bool ClearDraft { get; } = clearDraft;

    //public MessageBoxId MessageBoxId { get; }
    public int MessageId { get; } = messageId;

    //public int Pts { get; }
    public long OwnerPeerId { get; } = ownerPeerId;
    public Peer ToPeer { get; } = toPeer;
}
