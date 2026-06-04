using MyTelegram.ReadModel;

namespace MyTelegram.Queries.StarGift;

public class GetResaleStarGiftByCollectibleIdQuery(long collectibleId) : IQuery<IStarGiftReadModel?>
{
    public long CollectibleId { get; } = collectibleId;
}
