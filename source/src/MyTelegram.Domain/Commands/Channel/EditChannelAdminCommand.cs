namespace MyTelegram.Domain.Commands.Channel;

public class EditChannelAdminCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long promotedBy,
    bool canEdit,
    long userId,
    bool isBot,
    bool isChannelMember,
    ChatAdminRights adminRights,
    string rank,
    int date)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public ChatAdminRights AdminRights { get; } = adminRights;
    public bool CanEdit { get; } = canEdit;
    public long PromotedBy { get; } = promotedBy;
    public string Rank { get; } = rank;
    public int Date { get; } = date;
    public long UserId { get; } = userId;
    public bool IsBot { get; } = isBot;
    public bool IsChannelMember { get; } = isChannelMember;
}
