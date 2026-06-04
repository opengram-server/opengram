namespace MyTelegram.Domain.Commands.Dialog;

public class ReadChannelInboxMessageCommand(
    DialogId aggregateId,
    RequestInfo requestInfo,
    long readerUserId,
    long channelId,
    int maxId,
    long senderUserId,
    int? topMsgId)
    : RequestCommand2<DialogAggregate, DialogId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MaxId { get; } = maxId;
    public long SenderUserId { get; } = senderUserId;
    public int? TopMsgId { get; } = topMsgId;

    public long ReaderUserId { get; } = readerUserId;
}
