namespace MyTelegram.Domain.Commands.Temp;

public class StartInviteToChannelCommand(TempId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    bool isBroadcast,
    bool hasLink,
    long inviterId,
    int channelHistoryMinId,
    int maxMessageId,
    List<long> memberUserIds,
    List<long> botUserIds,
    ChatJoinType chatJoinType
    ) : RequestCommand2<TempAggregate, TempId, IExecutionResult>(aggregateId, requestInfo)
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
