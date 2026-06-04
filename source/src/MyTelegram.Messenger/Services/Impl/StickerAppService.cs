using Microsoft.Extensions.Logging;
using MyTelegram.Queries;

namespace MyTelegram.Messenger.Services.Impl;

public class StickerAppService : BaseAppService, IStickerAppService
{
    private readonly IQueryProcessor _queryProcessor;
    private readonly ILogger<StickerAppService> _logger;

    public StickerAppService(
        IQueryProcessor queryProcessor,
        ILogger<StickerAppService> logger)
    {
        _queryProcessor = queryProcessor;
        _logger = logger;
    }

    public async Task<long?> GetRandomFeaturedStickerIdAsync(string? emoji = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Optimization: If emoji is provided, use direct DB query to avoid loading all sticker packs
            if (!string.IsNullOrEmpty(emoji))
            {
                var stickerId = await _queryProcessor.ProcessAsync(
                    new GetRandomStickerByEmojiQuery(emoji),
                    cancellationToken);

                if (stickerId.HasValue)
                {
                    return stickerId;
                }
                
                _logger.LogWarning("No sticker found matching emoji {Emoji} in DB via direct query. Falling back to checking featured packs...", emoji);
                
                // Fallback: Check random featured packs (limit to 5 to avoid "downloading all")
                var fallbackFeaturedSets = await _queryProcessor.ProcessAsync(
                    new GetFeaturedStickerSetsQuery(StickerSetType.Regular),
                    cancellationToken);

                if (fallbackFeaturedSets.Count > 0)
                {
                    var shuffledSets = fallbackFeaturedSets.OrderBy(x => Random.Shared.Next()).Take(5).ToList();
                    foreach (var fallbackRandomSet in shuffledSets)
                    {
                        if (fallbackRandomSet.StickerDocumentIds.Count == 0) continue;
                        try 
                        {
                            var documents = await _queryProcessor.ProcessAsync(
                                new GetDocumentsByIdsQuery(fallbackRandomSet.StickerDocumentIds),
                                cancellationToken);

                            var matchingDocuments = documents.Where(d => 
                            {
                                var stickerAttr = d.Attributes2?.FirstOrDefault(a => a is MyTelegram.Schema.TDocumentAttributeSticker) as MyTelegram.Schema.TDocumentAttributeSticker;
                                return stickerAttr != null && (stickerAttr.Alt == emoji || stickerAttr.Alt == emoji + "\uFE0F");
                            }).ToList();

                            if (matchingDocuments.Count > 0)
                            {
                                var randomMatch = matchingDocuments[Random.Shared.Next(matchingDocuments.Count)];
                                _logger.LogInformation("Found sticker {StickerId} matching emoji {Emoji} in set {SetName} (Fallback)", 
                                    randomMatch.DocumentId, emoji, fallbackRandomSet.ShortName);
                                return randomMatch.DocumentId;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to check documents for set {SetName} in fallback", fallbackRandomSet.ShortName);
                        }
                    }
                }
                
                _logger.LogWarning("No sticker found matching emoji {Emoji} in fallback check", emoji);
                return null;
            }

            // Get featured regular sticker sets
            var featuredSets = await _queryProcessor.ProcessAsync(
                new GetFeaturedStickerSetsQuery(StickerSetType.Regular),
                cancellationToken);

            if (featuredSets.Count == 0)
            {
                _logger.LogWarning("No featured sticker sets found");
                return null;
            }

            // Get a random sticker set
            var setsList = featuredSets.ToList();
            _logger.LogInformation("Found {Count} featured sticker sets", setsList.Count);
            
            var randomSetIndex = Random.Shared.Next(setsList.Count);
            var randomSet = setsList[randomSetIndex];
            _logger.LogInformation("Selected random set index {Index}: {SetName} (Count: {StickerCount})", 
                randomSetIndex, randomSet.ShortName, randomSet.StickerDocumentIds.Count);

            // Get random sticker from the set
            if (randomSet.StickerDocumentIds.Count > 0)
            {
                var randomStickerIndex = Random.Shared.Next(randomSet.StickerDocumentIds.Count);
                var randomStickerId = randomSet.StickerDocumentIds[randomStickerIndex];
                
                _logger.LogInformation("Selected random sticker index {Index} with ID {StickerId} from set {SetName}",
                    randomStickerIndex, randomStickerId, randomSet.ShortName);
                return randomStickerId;
            }

            _logger.LogWarning("Random sticker set {SetName} has no st ickers", randomSet.ShortName);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get random featured sticker");
            return null;
        }
    }
}
