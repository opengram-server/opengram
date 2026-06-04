namespace MyTelegram.Domain.Events.PushUpdates;

public class EncryptedPushUpdatesCreatedEvent(
    long inboxOwnerPeerId,
    byte[] data,
    int qts,
    long inboxOwnerPermAuthKeyId,
    int date)
    : AggregateEvent<PushUpdatesAggregate, PushUpdatesId>
{
    public byte[] Data { get; } = data;
    public int Date { get; } = date;
    public long InboxOwnerPermAuthKeyId { get; } = inboxOwnerPermAuthKeyId;
    public long InboxOwnerPeerId { get; } = inboxOwnerPeerId;
    public int Qts { get; } = qts;
}
