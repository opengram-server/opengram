namespace MyTelegram.Domain.Events.Pts;

public class PtsCreatedEvent(
    RequestInfo requestInfo,
    long peerId,
    int pts,
    int qts,
    int unreadCount,
    int date)
    : RequestAggregateEvent2<PtsAggregate, PtsId>(requestInfo)
{
    public int Date { get; } = date;

    public long PeerId { get; } = peerId;
    public int Pts { get; } = pts;
    public int Qts { get; } = qts;
    public int UnreadCount { get; } = unreadCount;
}
