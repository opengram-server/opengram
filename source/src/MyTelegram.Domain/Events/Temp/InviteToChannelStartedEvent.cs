namespace MyTelegram.Domain.Events.Temp;

public class InviteToChannelStartedEvent(RequestInfo requestInfo,
    long channelId,
    bool isBroadcast,
    bool hasLink,
    long inviterId,
    int channelHistoryMinId,
    int maxMessageId,
    List<long> memberUserIds,
    List<long> botUserIds,
    ChatJoinType chatJoinType
) : RequestAggregateEvent2<TempAggregate, TempId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public bool IsBroadcast { get; } = isBroadcast;
    public bool HasLink { get; } = hasLink;
    public long InviterId { get; } = inviterId;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
    public int MaxMessageId { get; } = maxMessageId;
    public List<long> MemberUserIds { get; } = memberUserIds;
    public List<long> BotUserIds { get; } = botUserIds;
    public ChatJoinType ChatJoinType { get; } = chatJoinType;
}