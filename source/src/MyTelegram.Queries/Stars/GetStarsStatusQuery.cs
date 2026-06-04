using MyTelegram.Domain.Shared.Stars;

namespace MyTelegram.Queries.Stars;

public class GetStarsStatusQuery(long peerId, bool isTon = false) : IQuery<StarsStatus?>
{
    public long PeerId { get; } = peerId;
    public bool IsTon { get; } = isTon;
}
