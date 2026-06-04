namespace MyTelegram.Domain.Events.Channel;

public class ReadChannelLatestNoneBotOutboxMessageEvent(
    RequestInfo requestInfo,
    long latestNoneBotSenderPeerId,
    int latestNoneBotSenderMessageId,
    string sourceCommandId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public int LatestNoneBotSenderMessageId { get; } = latestNoneBotSenderMessageId;

    public long LatestNoneBotSenderPeerId { get; } = latestNoneBotSenderPeerId;
    public string SourceCommandId { get; } = sourceCommandId;
}
