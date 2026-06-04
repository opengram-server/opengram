namespace MyTelegram.Domain.Commands.Pts;

public class CreatePtsCommand(
    PtsId aggregateId,
    RequestInfo requestInfo,
    long peerId,
    int pts,
    int qts,
    int unreadCount,
    int date)
    : RequestCommand2<PtsAggregate, PtsId, IExecutionResult>(aggregateId, requestInfo)
{
    public int Date { get; } = date;
    public long PeerId { get; } = peerId;
    public int Pts { get; } = pts;
    public int Qts { get; } = qts;
    public int UnreadCount { get; } = unreadCount;

    protected override IEnumerable<byte[]> GetSourceIdComponents()
    {
        yield return BitConverter.GetBytes(PeerId);
    }
}
