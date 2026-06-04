namespace MyTelegram.Domain.Events.Channel;

public class JoinChannelRequestUpdatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long userId,
    bool approved,
    long? inviteId,
    int topMessageId,
    int channelHistoryMinId,
    bool broadcast) : RequestAggregateEvent2<JoinChannelAggregate, JoinChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long UserId { get; } = userId;
    public bool Approved { get; } = approved;
    public long? InviteId { get; } = inviteId;
    public int TopMessageId { get; } = topMessageId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
    public bool Broadcast { get; } = broadcast;
}