namespace MyTelegram.Domain.Sagas.Snapshots;

public class ReadHistorySagaSnapshot(
    RequestInfo requestInfo,
    long readerUserId,
    int readerMessageId,
    int readerPts,
    bool senderIsBot,
    long senderPeerId,
    int senderPts,
    int senderMessageId,
    Peer readerToPeer,
    bool isOut,
    bool readHistoryCompleted,
    bool outboxPtsIncremented,
    bool inboxPtsIncremented,
    bool latestNoneBotOutboxHasRead,
    bool needReadLatestNoneBotOutboxMessage,
    string sourceCommandId,
    Guid correlationId)
    : ISnapshot
{
    public Guid CorrelationId { get; } = correlationId;
    public bool InboxPtsIncremented { get; } = inboxPtsIncremented;
    public bool IsOut { get; } = isOut;
    public bool LatestNoneBotOutboxHasRead { get; } = latestNoneBotOutboxHasRead;
    public bool NeedReadLatestNoneBotOutboxMessage { get; } = needReadLatestNoneBotOutboxMessage;
    public bool OutboxPtsIncremented { get; } = outboxPtsIncremented;
    public int ReaderMessageId { get; } = readerMessageId;
    public int ReaderPts { get; } = readerPts;

    public Peer ReaderToPeer { get; } = readerToPeer;

    public RequestInfo RequestInfo { get; } = requestInfo;
    public long ReaderUserId { get; } = readerUserId;
    public bool ReadHistoryCompleted { get; } = readHistoryCompleted;

    public bool SenderIsBot { get; } = senderIsBot;

    public int SenderMessageId { get; } = senderMessageId;
    public int SenderPts { get; } = senderPts;

    public long SenderPeerId { get; } = senderPeerId;
    public string SourceCommandId { get; } = sourceCommandId;
}
