namespace MyTelegram.Domain.Events.Messaging;

public class OtherPartyMessageDeletedEvent(
    RequestInfo requestInfo,
    long ownerPeerId,
    int messageId)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public int MessageId { get; } = messageId;

    public long OwnerPeerId { get; } = ownerPeerId;
}