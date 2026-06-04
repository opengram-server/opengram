namespace MyTelegram.Domain.Sagas.Snapshots;

public class UpdatePinnedMessageSagaSnapshot(
    long reqMsgId,
    long selfAuthKeyId,
    PeerType toPeerType,
    long toPeerId,
    bool needWaitForOutboxPinnedUpdated,
    int inboxCount,
    int replyToMsgId,
    int updatedInboxCount,
    long startUpdatePinnedOwnerPeerId,
    long selfUserId,
    long randomId,
    string? messageActionData,
    long senderPeerId,
    int senderMessageId,
    bool pinned,
    bool pmOneSide,
    bool silent,
    int date,
    Guid correlationId,
    Dictionary<long, PinnedMsgItem> updatePinItems,
    int pinnedMsgId)
    : ISnapshot
{
    // bool receiveOutboxPinnedUpdated,
    // ReceiveOutboxPinnedUpdated = receiveOutboxPinnedUpdated;

    public Guid CorrelationId { get; } = correlationId;
    public int Date { get; } = date;
    public int InboxCount { get; } = inboxCount;
    public string? MessageActionData { get; } = messageActionData;

    public bool NeedWaitForOutboxPinnedUpdated { get; } = needWaitForOutboxPinnedUpdated;
    public bool Pinned { get; } = pinned;

    public int PinnedMsgId { get; } = pinnedMsgId;
    public bool PmOneSide { get; } = pmOneSide;
    public long RandomId { get; } = randomId;

    // public bool ReceiveOutboxPinnedUpdated { get; }
    public int ReplyToMsgId { get; } = replyToMsgId;

    public long ReqMsgId { get; } = reqMsgId;
    public long SelfAuthKeyId { get; } = selfAuthKeyId;
    public long SelfUserId { get; } = selfUserId;
    public int SenderMessageId { get; } = senderMessageId;
    public long SenderPeerId { get; } = senderPeerId;
    public bool Silent { get; } = silent;
    public long StartUpdatePinnedOwnerPeerId { get; } = startUpdatePinnedOwnerPeerId;
    public long ToPeerId { get; } = toPeerId;

    public PeerType ToPeerType { get; } = toPeerType;
    public int UpdatedInboxCount { get; } = updatedInboxCount;

    public Dictionary<long, PinnedMsgItem> UpdatePinItems { get; } = updatePinItems;
}
