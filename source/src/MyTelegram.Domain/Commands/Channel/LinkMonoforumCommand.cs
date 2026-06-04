namespace MyTelegram.Domain.Commands.Channel;

public class LinkMonoforumCommand(
    ChannelId aggregateId, 
    RequestInfo requestInfo, 
    long linkedMonoforumId,
    bool isMonoforum,
    bool broadcastMessagesAllowed)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long LinkedMonoforumId { get; } = linkedMonoforumId;
    public bool IsMonoforum { get; } = isMonoforum;
    public bool BroadcastMessagesAllowed { get; } = broadcastMessagesAllowed;
}
