namespace MyTelegram.Domain.Sagas.Events;

public class ReadHistoryCompletedSagaEvent(
    RequestInfo requestInfo,
    bool senderIsBot,
    long readerUserId,
    int readerMessageId,
    int readerPts,
    Peer readerToPeer,
    long senderPeerId,
    int senderPts,
    int senderMessageId,
    bool isOut,
    bool outboxAlreadyRead,
    string sourceCommandId)
    : RequestAggregateEvent2<ReadHistorySaga, ReadHistorySagaId>(requestInfo)
{
    public bool IsOut { get; } = isOut;
    public bool OutboxAlreadyRead { get; } = outboxAlreadyRead;
    public int ReaderMessageId { get; } = readerMessageId;
    public int ReaderPts { get; } = readerPts;
    public Peer ReaderToPeer { get; } = readerToPeer;
    public long ReaderUserId { get; } = readerUserId;
    public bool SenderIsBot { get; } = senderIsBot;
    public int SenderMessageId { get; } = senderMessageId;
    public int SenderPts { get; } = senderPts;

    public long SenderPeerId { get; } = senderPeerId;
    public string SourceCommandId { get; } = sourceCommandId;
}
