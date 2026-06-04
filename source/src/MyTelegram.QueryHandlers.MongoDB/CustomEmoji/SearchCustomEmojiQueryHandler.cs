using EventFlow.ReadStores;
using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.CustomEmoji;

public class SearchCustomEmojiQueryHandler :
    IQueryHandler<SearchCustomEmojiQuery, IReadOnlyList<ICustomEmojiReadModel>>
{
    private readonly IReadModelStore<CustomEmojiReadModel> _customEmojiStore;
    private readonly IReadModelStore<EmojiPackReadModel> _emojiPackStore;
    private readonly ILogger<SearchCustomEmojiQueryHandler> _logger;

    public SearchCustomEmojiQueryHandler(
        IReadModelStore<CustomEmojiReadModel> customEmojiStore,
        IReadModelStore<EmojiPackReadModel> emojiPackStore,
        ILogger<SearchCustomEmojiQueryHandler> logger)
    {
        _customEmojiStore = customEmojiStore;
        _emojiPackStore = emojiPackStore;
        _logger = logger;
    }

    public async Task<IReadOnlyList<ICustomEmojiReadModel>> ExecuteQueryAsync(
        SearchCustomEmojiQuery query,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query.Emoticon))
        {
            _logger.LogWarning("Empty emoticon provided for search");
            return Array.Empty<ICustomEmojiReadModel>();
        }

        _logger.LogInformation(
            "Searching custom emoji for emoticon '{Emoticon}', user {UserId}, premium: {IsPremium}",
            query.Emoticon,
            query.RequesterId,
            query.IsPremium);

        // Простая реализация - просто возвращаем пустой список
        // В production нужна полная реализация через MongoDB query
        _logger.LogWarning("SearchCustomEmoji not fully implemented yet");
        
        // TODO: Implement proper MongoDB query
        // Пока возвращаем пустой список
        return Array.Empty<ICustomEmojiReadModel>();
        
        /*
        // Полная реализация требует прямого доступа к MongoDB
        // через IMongoDatabase или специальный query handler
        var packs = await _emojiPackStore
            .GetAsync(...);
        
        var documentIds = packs
            .SelectMany(p => p.DocumentIds)
            .Distinct()
            .ToList();
        
        var emojis = await _customEmojiStore
            .GetAsync(...);
        
        var filtered = emojis.Where(e => query.IsPremium || e.IsFree).ToList();
        */

    }
}
