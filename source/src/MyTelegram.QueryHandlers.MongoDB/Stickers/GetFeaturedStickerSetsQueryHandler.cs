using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetFeaturedStickerSetsQueryHandler(IQueryOnlyReadModelStore<StickerSetReadModel> store)
    : IQueryHandler<GetFeaturedStickerSetsQuery, IReadOnlyCollection<IStickerSetReadModel>>
{
    public async Task<IReadOnlyCollection<IStickerSetReadModel>> ExecuteQueryAsync(
        GetFeaturedStickerSetsQuery query,
        CancellationToken cancellationToken)
    {
        var stickerSets = await store.FindAsync(
            x => x.IsFeatured && x.StickerSetType == query.StickerSetType,
            cancellationToken: cancellationToken);

        // Sort by FeaturedOrder
        return stickerSets.OrderBy(x => x.FeaturedOrder).ToList();
    }
}
