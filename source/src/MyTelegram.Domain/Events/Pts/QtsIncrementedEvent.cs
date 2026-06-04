namespace MyTelegram.Domain.Events.Pts;

public class QtsIncrementedEvent(
    RequestInfo requestInfo,
    long peerId,
    int qts,
    string encryptedMessageBoxId)
    : RequestAggregateEvent2<PtsAggregate, PtsId>(requestInfo)
{
    public string EncryptedMessageBoxId { get; } = encryptedMessageBoxId;

    public long PeerId { get; } = peerId;
    public int Qts { get; } = qts;
}
