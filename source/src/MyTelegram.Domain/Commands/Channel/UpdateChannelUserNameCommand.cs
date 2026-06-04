namespace MyTelegram.Domain.Commands.Channel;

public class UpdateChannelUserNameCommand(
    ChannelId aggregateId,
    RequestInfo requestInfo,
    long channelId,
    string userName)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo) //,
//IHasCorrelationId
{
    public long ChannelId { get; } = channelId;
    public string UserName { get; } = userName;
}
