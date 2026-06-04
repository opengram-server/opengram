namespace MyTelegram.Domain.Events.Channel;

public class CheckChannelStateCompletedEvent(
    RequestInfo requestInfo,
    long senderPeerId,
    int messageId,
    int date,
    bool post,
    int? views,
    IReadOnlyList<long> botUidList,
    long? linkedChannelId)
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    public long SenderPeerId { get; } = senderPeerId;
    public int MessageId { get; } = messageId;
    public int Date { get; } = date;
    public bool Post { get; } = post;
    public int? Views { get; } = views;
    public IReadOnlyList<long> BotUidList { get; } = botUidList;
    public long? LinkedChannelId { get; } = linkedChannelId;
}