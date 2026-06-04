namespace MyTelegram.Domain.Events.Channel;

public class ChannelMemberBannedRightsChangedEvent(
    RequestInfo requestInfo,
    long adminId,
    long channelId,
    long memberUserId,
    bool isBot,
    bool kicked,
    long kickedBy,
    bool left,
    bool banned,
    bool removedFromKicked,
    bool removedFromBanned,
    ChatBannedRights bannedRights)
    : RequestAggregateEvent2<ChannelMemberAggregate, ChannelMemberId>(requestInfo)
{
    public long AdminId { get; } = adminId;
    public ChatBannedRights BannedRights { get; } = bannedRights;
    public long ChannelId { get; } = channelId;
    public long MemberUserId { get; } = memberUserId;
    public bool IsBot { get; } = isBot;
    public bool Kicked { get; } = kicked;
    public long KickedBy { get; } = kickedBy;
    public bool Left { get; } = left;

    public bool Banned { get; } = banned;
    public bool RemovedFromKicked { get; } = removedFromKicked;
    public bool RemovedFromBanned { get; } = removedFromBanned;
}
