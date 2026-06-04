namespace MyTelegram.Domain.Events.Messaging;

public class InboxMessageHasReadEvent(
    RequestInfo requestInfo,
    long readerUid,
    int maxMessageId,
    long senderPeerId,
    int senderMessageId,
    Peer toPeer,
    bool isOut)
    : RequestAggregateEvent2<MessageAggregate, MessageId>(requestInfo)
{
    public long ReaderUid { get; } = readerUid;
    public int MaxMessageId { get; } = maxMessageId;
    public long SenderPeerId { get; } = senderPeerId;
    public int SenderMessageId { get; } = senderMessageId;
    public Peer ToPeer { get; } = toPeer;
    public bool IsOut { get; } = isOut;
}