namespace MyTelegram.Domain.Commands.ChatInvite;

public class EditChatInviteCommand(
    ChatInviteId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    long inviteId,
    string hash,
    string? newHash,
    long adminId,
    string? title,
    bool requestNeeded,
    int? startDate,
    int? expireDate,
    int? usageLimit,
    bool permanent,
    bool revoked)
    : RequestCommand2<ChatInviteAggregate, ChatInviteId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long InviteId { get; } = inviteId;
    public string Hash { get; } = hash;
    public string? NewHash { get; } = newHash;
    public long AdminId { get; } = adminId;
    public string? Title { get; } = title;
    public bool RequestNeeded { get; } = requestNeeded;
    public int? StartDate { get; } = startDate;
    public int? ExpireDate { get; } = expireDate;
    public int? UsageLimit { get; } = usageLimit;
    public bool Permanent { get; } = permanent;
    public bool Revoked { get; } = revoked;
}