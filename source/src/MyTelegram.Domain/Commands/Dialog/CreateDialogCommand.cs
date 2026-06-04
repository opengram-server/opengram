namespace MyTelegram.Domain.Commands.Dialog;

public class CreateDialogCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    long ownerId,
    Peer toPeer,
    int channelHistoryMinId,
    int topMessageId,
    int? ttlPeriod = null)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    //TopMessageBoxId = topMessageBoxId; 

    public int ChannelHistoryMinId { get; } = channelHistoryMinId;

    //public string TopMessageBoxId { get; } 

    public long OwnerId { get; } = ownerId;
    public Peer ToPeer { get; } = toPeer;
    public int TopMessageId { get; } = topMessageId;
    public int? TtlPeriod { get; } = ttlPeriod;
}
