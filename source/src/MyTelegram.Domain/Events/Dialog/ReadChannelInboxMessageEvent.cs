namespace MyTelegram.Domain.Events.Dialog;

public class ReadChannelInboxMessageEvent(
    RequestInfo requestInfo,
    long readerUserId,
    long channelId,
    int maxId,
    long senderUserId,
    int? topMsgId)
    : RequestAggregateEvent2<DialogAggregate, DialogId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int MaxId { get; } = maxId;
    public long SenderUserId { get; } = senderUserId;
    public int? TopMsgId { get; } = topMsgId;

    public long ReaderUserId { get; } = readerUserId;
}