namespace MyTelegram.Domain.Events.Channel;

public class JoinChannelRequestCreatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long userId,
    int date,
    long? inviteId) : RequestAggregateEvent2<JoinChannelAggregate, JoinChannelId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long UserId { get; } = userId;
    public int Date { get; } = date;
    public long? InviteId { get; } = inviteId;
}