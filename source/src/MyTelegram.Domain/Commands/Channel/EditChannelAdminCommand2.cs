namespace MyTelegram.Domain.Commands.Channel;

public class
    EditChannelAdminCommand2(ChannelMemberId aggregateId, RequestInfo requestInfo, long channelId, long userId, int adminRights, string rank) : RequestCommand2<ChannelMemberAggregate, ChannelMemberId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long UserId { get; } = userId;
    public int AdminRights { get; } = adminRights;
    public string Rank { get; } = rank;
}