using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;
using System.Linq.Expressions;

namespace MyTelegram.QueryHandlers.MongoDB.Stickers;

public class GetInstalledStickerSetsQueryHandler(
    IQueryOnlyReadModelStore<InstalledStickerSetReadModel> store,
    ILogger<GetInstalledStickerSetsQueryHandler> logger)
    : IQueryHandler<GetInstalledStickerSetsQuery, IReadOnlyCollection<IInstalledStickerSetReadModel>>
{
    public async Task<IReadOnlyCollection<IInstalledStickerSetReadModel>> ExecuteQueryAsync(
        GetInstalledStickerSetsQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Loading installed sticker sets: UserId={UserId}, Type={Type}, SpecificIds={HasIds}",
            query.UserId,
            query.StickerSetType,
            query.StickerSetIds?.Any() ?? false);
        
        // Log all records in collection for debugging (only if needed)
        if (logger.IsEnabled(LogLevel.Debug))
        {
            try
            {
                var allRecords = await store.FindAsync(p => true, 0, 100, cancellationToken: cancellationToken);
                logger.LogDebug(
                    "Total records in InstalledStickerSetReadModel collection: {Count}",
                    allRecords.Count);
                
                foreach (var record in allRecords)
                {
                    logger.LogDebug(
                        "Record: Id={Id}, UserId={UserId}, StickerSetId={StickerSetId}, Type={Type}",
                        record.Id,
                        record.UserId,
                        record.StickerSetId,
                        record.StickerSetType);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error reading all records from collection");
            }
        }
        
        Expression<Func<InstalledStickerSetReadModel, bool>> predicate = 
            p => p.UserId == query.UserId && p.StickerSetType == query.StickerSetType;
        
        logger.LogDebug(
            "Query predicate: UserId={UserId}, StickerSetType={Type}",
            query.UserId,
            query.StickerSetType);
        
        if (query.StickerSetIds != null && query.StickerSetIds.Any())
        {
            predicate = predicate.And(p => query.StickerSetIds.Contains(p.StickerSetId));
            logger.LogDebug(
                "Filtering by specific StickerSetIds: {Ids}",
                string.Join(", ", query.StickerSetIds));
        }
        
        var results = await store.FindAsync(predicate, 0, int.MaxValue, cancellationToken: cancellationToken);
        
        logger.LogInformation(
            "Found {Count} installed sticker sets for user {UserId}",
            results.Count,
            query.UserId);
        
        return results.ToList();
    }
}
