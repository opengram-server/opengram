namespace MyTelegram.Domain.Events.Channel;

public class StartInviteToChannelEvent(
    RequestInfo requestInfo,
    long channelId,
    long inviterId,
    IReadOnlyList<long> memberUidList,
    IReadOnlyList<long>? privacyRestrictedUserId,
    IReadOnlyList<long> botUidList,
    int date,
    int maxMessageId,
    int channelHistoryMinId,
    long randomId,
    string messageActionData,
    bool broadcast,
    bool hasLink
    )
    : RequestAggregateEvent2<ChannelAggregate, ChannelId>(requestInfo)
{
    //bool isBot,

    public IReadOnlyList<long> BotUidList { get; } = botUidList;
    public int ChannelHistoryMinId { get; } = channelHistoryMinId;
    public long ChannelId { get; } = channelId;
    public int Date { get; } = date;
    public long InviterId { get; } = inviterId;
    public int MaxMessageId { get; } = maxMessageId;
    public IReadOnlyList<long> MemberUidList { get; } = memberUidList;
    public IReadOnlyList<long>? PrivacyRestrictedUserId { get; } = privacyRestrictedUserId;
    public string MessageActionData { get; } = messageActionData;
    public bool Broadcast { get; } = broadcast;
    public bool HasLink { get; } = hasLink;
    public long RandomId { get; } = randomId;

    //public bool IsBot { get; } 
}
