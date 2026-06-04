namespace MyTelegram.Domain.Events.Messaging;

public class MessageReactionRemovedEvent : RequestAggregateEvent2<MessageAggregate, MessageId>
{
    public MessageReactionRemovedEvent(
        RequestInfo requestInfo,
        long ownerPeerId,
        int messageId,
        long senderUserId,
        IReaction reaction) : base(requestInfo)
    {
        OwnerPeerId = ownerPeerId;
        MessageId = messageId;
        SenderUserId = senderUserId;
        Reaction = reaction;
    }

    public long OwnerPeerId { get; }
    public int MessageId { get; }
    public long SenderUserId { get; }
    public IReaction Reaction { get; }
}
