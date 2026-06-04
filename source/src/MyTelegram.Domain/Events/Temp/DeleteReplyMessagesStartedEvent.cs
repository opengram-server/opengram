namespace MyTelegram.Domain.Events.Temp;

public class DeleteReplyMessagesStartedEvent(
    RequestInfo requestInfo,
    long channelId,
    List<int> messageIds,
    int newTopMessageId)
    : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public List<int> MessageIds { get; } = messageIds;
    public int NewTopMessageId { get; } = newTopMessageId;
}