namespace MyTelegram.Domain.Events.ChatInvite;

public class ChatInviteCreatedEvent(
    RequestInfo requestInfo,
    long channelId,
    long inviteId,
    string hash,
    long adminId,
    string? title,
    bool requestNeeded,
    int? startDate,
    int? expireDate,
    int? usageLimit,
    bool permanent,
    int date,
    bool isBroadcast
    )
    : RequestAggregateEvent2<ChatInviteAggregate, ChatInviteId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long InviteId { get; } = inviteId;
    public string Hash { get; } = hash;
    public long AdminId { get; } = adminId;
    public string? Title { get; } = title;
    public bool RequestNeeded { get; } = requestNeeded;
    public int? StartDate { get; } = startDate;
    public int? ExpireDate { get; } = expireDate;
    public int? UsageLimit { get; } = usageLimit;
    public bool Permanent { get; } = permanent;
    public int Date { get; } = date;
    public bool IsBroadcast { get; } = isBroadcast;
}