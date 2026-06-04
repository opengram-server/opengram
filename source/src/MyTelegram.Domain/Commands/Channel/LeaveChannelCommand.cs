namespace MyTelegram.Domain.Commands.Channel;

public class LeaveChannelCommand(
    ChannelMemberId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    long memberUserId)
    : RequestCommand2<ChannelMemberAggregate, ChannelMemberId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long MemberUserId { get; } = memberUserId;
}