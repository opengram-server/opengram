namespace MyTelegram.Domain.Aggregates.Pts;

public class PtsSnapshot(
    long peerId,
    int pts,
    int qts,
    int unreadCount,
    int date,
    long globalSeqNo,
    long permAuthKeyId)
    : ISnapshot
{
    public int Date { get; } = date;
    public long GlobalSeqNo { get; } = globalSeqNo;
    public long PermAuthKeyId { get; } = permAuthKeyId;

    public long PeerId { get; } = peerId;
    public int Pts { get; } = pts;
    public int Qts { get; } = qts;
    public int UnreadCount { get; } = unreadCount;
}
