namespace MyTelegram.Domain.Commands.Messaging;

public class RemoveReactionCommand : RequestCommand2<MessageAggregate, MessageId, IExecutionResult>
{
    public RemoveReactionCommand(
        MessageId aggregateId,
        RequestInfo requestInfo,
        long senderUserId,
        IReaction reaction) : base(aggregateId, requestInfo)
    {
        SenderUserId = senderUserId;
        Reaction = reaction;
    }

    public long SenderUserId { get; }
    public IReaction Reaction { get; }
}
