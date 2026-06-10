using MyTelegram.Domain.Events.EncryptedChat;

namespace MyTelegram.Domain.Aggregates.EncryptedChat;

public class EncryptedChatState : AggregateState<EncryptedChatAggregate, EncryptedChatId, EncryptedChatState>,
    IApply<EncryptionRequestedEvent>,
    IApply<EncryptionAcceptedEvent>,
    IApply<EncryptionDiscardedEvent>,
    IApply<EncryptedMessageSentEvent>
{
    public int ChatId { get; private set; }
    public long AccessHash { get; private set; }
    public long AdminId { get; private set; }
    public long ParticipantId { get; private set; }
    public long AdminPermAuthKeyId { get; private set; }
    public long ParticipantPermAuthKeyId { get; private set; }
    public EncryptedChatStatus Status { get; private set; }

    // DH exchange data
    public byte[]? GA { get; private set; }
    public byte[]? GB { get; private set; }
    public long KeyFingerprint { get; private set; }

    // Timestamps
    public int Date { get; private set; }

    public void Apply(EncryptionRequestedEvent aggregateEvent)
    {
        ChatId = aggregateEvent.ChatId;
        AccessHash = aggregateEvent.AccessHash;
        AdminId = aggregateEvent.AdminId;
        ParticipantId = aggregateEvent.ParticipantId;
        AdminPermAuthKeyId = aggregateEvent.AdminPermAuthKeyId;
        GA = aggregateEvent.GA;
        Date = aggregateEvent.Date;
        Status = EncryptedChatStatus.Requested;
    }

    public void Apply(EncryptionAcceptedEvent aggregateEvent)
    {
        GB = aggregateEvent.GB;
        KeyFingerprint = aggregateEvent.KeyFingerprint;
        ParticipantPermAuthKeyId = aggregateEvent.ParticipantPermAuthKeyId;
        Status = EncryptedChatStatus.Accepted;
    }

    public void Apply(EncryptionDiscardedEvent aggregateEvent)
    {
        Status = EncryptedChatStatus.Discarded;
    }

    public void Apply(EncryptedMessageSentEvent aggregateEvent)
    {
        // Messages are relayed, no state change needed
    }
}

public enum EncryptedChatStatus
{
    Empty = 0,
    Requested = 1,
    Accepted = 2,
    Discarded = 3
}
