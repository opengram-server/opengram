namespace MyTelegram.Domain.Commands.Channel;

public class UpdatePaidMessagesPriceCommand(ChannelId aggregateId, RequestInfo requestInfo, long? sendPaidMessagesStars, bool broadcastMessagesAllowed, long? linkedMonoforumId = null)
    : RequestCommand2<ChannelAggregate, ChannelId, IExecutionResult>(aggregateId, requestInfo)
{
    public long? SendPaidMessagesStars { get; } = sendPaidMessagesStars;
    public bool BroadcastMessagesAllowed { get; } = broadcastMessagesAllowed;
    public long? LinkedMonoforumId { get; } = linkedMonoforumId;
}
