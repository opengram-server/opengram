namespace MyTelegram.Domain.Events.Messaging;

public class MessagePinnedUpdatedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId,
    bool pinned,
    Peer toPeer,
    bool post
    ) : AggregateEvent<MessageAggregate, MessageId>, IHasRequestInfo
{
    public long OwnerPeerId { get; } = ownerPeerId;
    public int MessageId { get; } = messageId;
    public bool Pinned { get; } = pinned;
    public Peer ToPeer { get; } = toPeer;
    public bool Post { get; } = post;
    public RequestInfo RequestInfo { get; } = requestInfo;
}