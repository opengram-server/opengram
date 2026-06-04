using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetStickerSetsByIdListQueryHandler(IQueryOnlyReadModelStore<StickerSetReadModel> store)
    : IQueryHandler<GetStickerSetsByIdListQuery, IReadOnlyCollection<IStickerSetReadModel>>
{
    public async Task<IReadOnlyCollection<IStickerSetReadModel>> ExecuteQueryAsync(
        GetStickerSetsByIdListQuery query,
        CancellationToken cancellationToken)
    {
        if (query.StickerSetIds == null || !query.StickerSetIds.Any())
        {
            return Array.Empty<IStickerSetReadModel>();
        }

        var result = await store.FindAsync(
            p => query.StickerSetIds.Contains(p.StickerSetId));

        return result.ToList();
    }
}
