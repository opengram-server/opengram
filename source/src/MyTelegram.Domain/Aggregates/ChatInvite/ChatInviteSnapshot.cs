namespace MyTelegram.Domain.Aggregates.ChatInvite;

public class ChatInviteSnapshot(
    long channelId,
    long inviteId,
    string hash,
    long adminId,
    string? title,
    bool requestNeeded,
    int date,
    int? startDate,
    int? expireDate,
    int? usageLimit,
    bool permanent,
    bool revoked,
    int? usage,
    int? requested,
    bool isBroadcast
    )
    : ISnapshot
{
    public long ChannelId { get; } = channelId;
    public long InviteId { get; } = inviteId;
    public string Hash { get; } = hash;
    public long AdminId { get; } = adminId;
    public string? Title { get; } = title;
    public bool RequestNeeded { get; } = requestNeeded;
    public int Date { get; } = date;
    public int? StartDate { get; } = startDate;
    public int? ExpireDate { get; } = expireDate;
    public int? UsageLimit { get; } = usageLimit;
    public bool Permanent { get; } = permanent;
    public bool Revoked { get; } = revoked;
    public int? Usage { get; } = usage;
    public int? Requested { get; } = requested;
    public bool IsBroadcast { get; } = isBroadcast;
}