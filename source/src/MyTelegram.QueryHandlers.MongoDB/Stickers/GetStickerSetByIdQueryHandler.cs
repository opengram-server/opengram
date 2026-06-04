using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetStickerSetByIdQueryHandler(IQueryOnlyReadModelStore<StickerSetReadModel> store)
    : IQueryHandler<GetStickerSetByIdQuery, IStickerSetReadModel?>
{
    public async Task<IStickerSetReadModel?> ExecuteQueryAsync(
        GetStickerSetByIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.StickerSetId == query.StickerSetId, cancellationToken);
    }
}
