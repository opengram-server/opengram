namespace MyTelegram.Domain.Commands.Dialog;

public class ReceiveInboxMessageCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    int messageId,
    long ownerPeerId,
    Peer toPeer,
    int? senderDefaultHistoryTTL = null)
    : /*Request*/Command<DialogAggregate, DialogId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    //long reqMsgId,
    //MessageBoxId messageBoxId,
    //int pts,
    //MessageBoxId = messageBoxId;
    //Pts = pts;

    //public MessageBoxId MessageBoxId { get; }
    public int MessageId { get; } = messageId;
    public long OwnerPeerId { get; } = ownerPeerId;

    //public int Pts { get; }
    public Peer ToPeer { get; } = toPeer;
    public RequestInfo RequestInfo { get; } = requestInfo;
    public int? SenderDefaultHistoryTTL { get; } = senderDefaultHistoryTTL;
}
