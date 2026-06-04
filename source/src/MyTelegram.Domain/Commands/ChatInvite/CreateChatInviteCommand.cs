namespace MyTelegram.Domain.Commands.ChatInvite;

public class CreateChatInviteCommand(
    ChatInviteId aggregateId,
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
    : RequestCommand2<ChatInviteAggregate, ChatInviteId, IExecutionResult>(aggregateId, requestInfo)
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