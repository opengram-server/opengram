namespace MyTelegram.Domain.Sagas;

public class DeletedBoxItem(
    long ownerPeerId,
    int pts,
    int ptsCount,
    IReadOnlyList<int> deletedMessageIdList)
{
    public IReadOnlyList<int> DeletedMessageIdList { get; } = deletedMessageIdList;

    public long OwnerPeerId { get; } = ownerPeerId;
    public int Pts { get; } = pts;
    public int PtsCount { get; } = ptsCount;
}
