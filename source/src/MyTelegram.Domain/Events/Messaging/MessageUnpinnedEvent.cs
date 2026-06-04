namespace MyTelegram.Domain.Events.Messaging;

public class MessageUnpinnedEvent(RequestInfo requestInfo, long ownerPeerId, int messageId)
    : AggregateEvent<MessageAggregate, MessageId>, IHasRequestInfo
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public RequestInfo RequestInfo { get; } = requestInfo;
}