namespace MyTelegram.Domain.Commands.Messaging;

public class AddReactionCommand : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>
{
    public AddReactionCommand(
        MessageId aggregateId,
        RequestInfo requestInfo,
        long senderUserId,
        Peer senderPeer,
        IReaction reaction,
        bool big,
        bool addToRecent,
        int date,
        int count = 1) : base(aggregateId, requestInfo)
    {
        SenderUserId = senderUserId;
        SenderPeer = senderPeer;
        Reaction = reaction;
        Big = big;
        AddToRecent = addToRecent;
        Date = date;
        Count = count;
    }

    public long SenderUserId { get; }
    public Peer SenderPeer { get; }
    public IReaction Reaction { get; }
    public bool Big { get; }
    public bool AddToRecent { get; }
    public int Date { get; }
    public int Count { get; }
}
