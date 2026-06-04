namespace MyTelegram.Domain.Events.Messaging;

public class MessageReactionsUpdatedEvent : AggregateEvent<MessageAggregate, MessageId>
{
    public MessageReactionsUpdatedEvent(
        long ownerPeerId,
        int messageId,
        List<ReactionCount>? reactions,
        List<MessagePeerReaction>? recentReactions)
    {
        OwnerPeerId = ownerPeerId;
        MessageId = messageId;
        Reactions = reactions;
        RecentReactions = recentReactions;
    }

    public long OwnerPeerId { get; }
    public int MessageId { get; }
    public List<ReactionCount>? Reactions { get; }
    public List<MessagePeerReaction>? RecentReactions { get; }
}
