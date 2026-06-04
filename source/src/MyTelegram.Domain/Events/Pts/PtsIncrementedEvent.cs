namespace MyTelegram.Domain.Events.Pts;

public class PtsIncrementedEvent(
    RequestInfo requestInfo,
    long peerId,
    int pts,
    PtsChangeReason reason,
    string messageBoxId)
    : RequestAggregateEvent2<PtsAggregate, PtsId>(requestInfo)
{
    public string MessageBoxId { get; } = messageBoxId;

    public long PeerId { get; } = peerId;

    /// <summary>
    ///     the new pts
    /// </summary>
    public int Pts { get; } = pts;

    public PtsChangeReason Reason { get; } = reason;
}
