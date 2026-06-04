namespace MyTelegram.Domain.Events.Channel;

public class ChannelMemberJoinedEvent(
    RequestInfo requestInfo,
    long channelId,
    long memberUserId,
    int date,
    bool isRejoin,
    bool isBot,
    bool isBroadcast
    )
    : RequestAggregateEvent2<ChannelMemberAggregate, ChannelMemberId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public int Date { get; } = date;
    public bool IsRejoin { get; } = isRejoin;
    public bool IsBot { get; } = isBot;
    public bool IsBroadcast { get; } = isBroadcast;
    public long MemberUserId { get; } = memberUserId;
}
