namespace MyTelegram.Domain.Events.ChatInvite;

public class ChatInviteEditedEvent(
    RequestInfo requestInfo,
    long channelId,
    long inviteId,
    string hash,
    string? newHash,
    long adminId,
    string? title,
    bool requestNeeded,
    int date,
    int? startDate,
    int? expireDate,
    int? usageLimit,
    bool permanent,
    bool revoked,
    int? requested,
    int? usage,
    bool isBroadcast
    )
    : RequestAggregateEvent2<ChatInviteAggregate, ChatInviteId>(requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long InviteId { get; } = inviteId;
    public string Hash { get; } = hash;
    public string? NewHash { get; } = newHash;
    public long AdminId { get; } = adminId;
    public string? Title { get; } = title;
    public bool RequestNeeded { get; } = requestNeeded;
    public int Date { get; } = date;
    public int? StartDate { get; } = startDate;
    public int? ExpireDate { get; } = expireDate;
    public int? UsageLimit { get; } = usageLimit;
    public bool Permanent { get; } = permanent;
    public bool Revoked { get; } = revoked;
    public int? Requested { get; } = requested;
    public int? Usage { get; } = usage;
    public bool IsBroadcast { get; } = isBroadcast;
}