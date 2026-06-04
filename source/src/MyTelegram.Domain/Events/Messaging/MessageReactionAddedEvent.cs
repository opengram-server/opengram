namespace MyTelegram.Domain.Events.Messaging;

public class MessageReactionAddedEvent : RequestAggregateEvent2<MessageAggregate, MessageId>
{
    public MessageReactionAddedEvent(
        RequestInfo requestInfo,
        long ownerPeerId,
        int messageId,
        long senderUserId,
        Peer senderPeer,
        IReaction reaction,
        bool big,
        bool addToRecent,
        int date,
        int count) : base(requestInfo)
    {
        OwnerPeerId = ownerPeerId;
        MessageId = messageId;
        SenderUserId = senderUserId;
        SenderPeer = senderPeer;
        Reaction = reaction;
        Big = big;
        AddToRecent = addToRecent;
        Date = date;
        Count = count;
    }

    public long OwnerPeerId { get; }
    public int MessageId { get; }
    public long SenderUserId { get; }
    public Peer SenderPeer { get; }
    public IReaction Reaction { get; }
    public bool Big { get; }
    public bool AddToRecent { get; }
    public int Date { get; }
    public int Count { get; }
}
