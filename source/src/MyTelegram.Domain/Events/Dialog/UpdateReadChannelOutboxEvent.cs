namespace MyTelegram.Domain.Events.Dialog;

public class UpdateReadChannelOutboxEvent(RequestInfo requestInfo, long messageSenderUserId, long channelId, int maxId)
    : AggregateEvent<DialogAggregate, DialogId>, IHasRequestInfo
{
    public long MessageSenderUserId { get; } = messageSenderUserId;
    public long ChannelId { get; } = channelId;
    public int MaxId { get; } = maxId;

    public RequestInfo RequestInfo { get; } = requestInfo;
}