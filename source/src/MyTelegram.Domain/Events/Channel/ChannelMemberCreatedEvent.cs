namespace MyTelegram.Domain.Events.Channel;

public class ChannelMemberCreatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long userId,
    long inviterId,
    int date,
    bool isRejoin,
    ChatBannedRights? bannedRights,
    bool isBot,
    long? chatInviteId,
    ChatJoinType chatJoinType,
    bool isBroadcast
    )
    : RequestAggregateEvent2<ChannelMemberAggregate, ChannelMemberId>(requestInfo)
{
    //long reqMsgId,

    public ChatBannedRights? BannedRights { get; } = bannedRights;
    public long ChannelId { get; } = channelId;
    public int Date { get; } = date;
    public long InviterId { get; } = inviterId;
    public bool IsBot { get; } = isBot;
    public long? ChatInviteId { get; } = chatInviteId;
    public ChatJoinType ChatJoinType { get; } = chatJoinType;
    public bool IsBroadcast { get; } = isBroadcast;
    public bool IsRejoin { get; } = isRejoin;
    public long UserId { get; } = userId;
}
