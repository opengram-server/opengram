using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetStickerSetByNameQueryHandler(IQueryOnlyReadModelStore<StickerSetReadModel> store)
    : IQueryHandler<GetStickerSetByNameQuery, IStickerSetReadModel?>
{
    public async Task<IStickerSetReadModel?> ExecuteQueryAsync(
        GetStickerSetByNameQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.ShortName == query.ShortName, cancellationToken);
    }
}
