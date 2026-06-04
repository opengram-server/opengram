namespace MyTelegram.Domain.Commands.Channel;

public class CreateJoinChannelRequestCommand(
    JoinChannelId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    long? inviteId) : RequestCommand2<JoinChannelAggregate, JoinChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long ChannelId { get; } = channelId;
    public long? InviteId { get; } = inviteId;
}