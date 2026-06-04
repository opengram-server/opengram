using MyTelegram.ReadModel.Impl;
using Microsoft.Extensions.Logging;
using MyTelegram.EventFlow.ReadStores;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetSavedStarGiftsQueryHandler(
    IQueryOnlyReadModelStore<StarGiftReadModel> store,
    ILogger<GetSavedStarGiftsQueryHandler> logger) 
    : IQueryHandler<GetSavedStarGiftsQuery, IReadOnlyCollection<IStarGiftReadModel>>
{
    public async Task<IReadOnlyCollection<IStarGiftReadModel>> ExecuteQueryAsync(GetSavedStarGiftsQuery query, CancellationToken cancellationToken)
    {
        logger.LogInformation("[GetSavedStarGiftsQueryHandler] UserId={UserId}, PeerId={PeerId}, ExcludeUnsaved={ExcludeUnsaved}, ExcludeSaved={ExcludeSaved}, ExcludeLimited={ExcludeLimited}, ExcludeUnlimited={ExcludeUnlimited}, ExcludeUnique={ExcludeUnique}, Offset={Offset}, Limit={Limit}",
            query.UserId, query.PeerId, query.ExcludeUnsaved, query.ExcludeSaved, query.ExcludeLimited, query.ExcludeUnlimited, query.ExcludeUnique, query.Offset, query.Limit);

        // Сначала считаем общее число подарков без лимита, чтобы понять, есть ли ещё записи
        var totalGifts = await store.CountAsync(
            x => x.ToUserId == query.UserId &&
                 (!query.PeerId.HasValue || x.FromUserId == query.PeerId.Value || x.ToPeerId == query.PeerId.Value) &&
                 (!query.ExcludeUnsaved || x.Saved) &&
                 (!query.ExcludeSaved || !x.Saved) &&
                 !x.Converted,
            cancellationToken: cancellationToken);
        
        logger.LogInformation("[GetSavedStarGiftsQueryHandler] Total gifts in DB for user {UserId}: {Total}", query.UserId, totalGifts);

        // Не фильтруем по Converted — нужны все подарки (и сохранённые, и нет)
        var result = await store.FindAsync(
            x => x.ToUserId == query.UserId &&
                 (!query.PeerId.HasValue || x.FromUserId == query.PeerId.Value || x.ToPeerId == query.PeerId.Value) &&
                 (!query.ExcludeUnsaved || x.Saved) &&
                 (!query.ExcludeSaved || !x.Saved) &&
                 // Фильтры ExcludeLimited, ExcludeUnlimited, ExcludeUnique требуют дополнительных полей в StarGiftReadModel.
                 // Пока их не применяем, так как для них нужны метаданные подарка.
                 !x.Converted,  // Не показываем конвертированные подарки
            skip: query.Offset,
            limit: query.Limit,
            sort: new SortOptions<StarGiftReadModel>(x => x.Date, SortType.Descending),  // Сортировка по дате по убыванию (сначала новые)
            cancellationToken: cancellationToken);

        logger.LogInformation("[GetSavedStarGiftsQueryHandler] Found {Count} gifts (out of {Total} total)", result.Count, totalGifts);
        
        return result;
    }
}
