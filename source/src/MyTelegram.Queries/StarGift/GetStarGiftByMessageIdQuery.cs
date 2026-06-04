using MyTelegram.ReadModel;

namespace MyTelegram.Queries.StarGift;

public class GetStarGiftByMessageIdQuery(long userId, int messageId, long? peerId = null, bool preferUpgraded = false) : IQuery<IStarGiftReadModel?>
{
    public long UserId { get; } = userId;
    public int MessageId { get; } = messageId;
    public long? PeerId { get; } = peerId;
    public bool PreferUpgraded { get; } = preferUpgraded; // True for resale, False for upgrade
}
