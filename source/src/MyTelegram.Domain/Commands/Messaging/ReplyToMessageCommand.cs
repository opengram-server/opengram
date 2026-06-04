namespace MyTelegram.Domain.Commands.Messaging;

public class ReplyToMessageCommand(MessageId aggregateId,
        RequestInfo requestInfo,
        Peer replierPeer,
        int repliesPts,
        int messageId)
    : Command<MessageAggregate, MessageId, IExecutionResult>(aggregateId), IHasRequestInfo
{
    public RequestInfo RequestInfo { get; } = requestInfo;
    public Peer ReplierPeer { get; } = replierPeer;
    public int MessageId { get; } = messageId;
    public int RepliesPts { get; } = repliesPts;
}