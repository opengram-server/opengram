using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MyTelegram.Messenger.Services.Impl;

public class FrozenSettingsService : IFrozenSettingsService
{
    private readonly ILogger<FrozenSettingsService> _logger;
    private readonly IOptionsMonitor<MyTelegramMessengerServerOptions> _options;
    private readonly IMongoDatabase _mongoDatabase;
    private long? _cachedSnowflakeEmojiId;
    private DateTime _lastRefresh = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public FrozenSettingsService(
        ILogger<FrozenSettingsService> logger,
        IOptionsMonitor<MyTelegramMessengerServerOptions> options,
        IMongoDatabase mongoDatabase)
    {
        _logger = logger;
        _options = options;
        _mongoDatabase = mongoDatabase;
    }

    public async Task<long?> GetSnowflakeEmojiIdAsync()
    {
        // Check cache first
        if (_cachedSnowflakeEmojiId.HasValue && 
            DateTime.UtcNow - _lastRefresh < _cacheExpiration)
        {
            return _cachedSnowflakeEmojiId;
        }

        // Refresh from MongoDB
        await RefreshSettingsAsync();
        return _cachedSnowflakeEmojiId;
    }

    public async Task RefreshSettingsAsync()
    {
        try
        {
            // First check appsettings.json
            var configuredId = _options.CurrentValue.FrozenAccountSnowflakeEmojiId;
            if (configuredId.HasValue && configuredId.Value > 0)
            {
                _cachedSnowflakeEmojiId = configuredId.Value;
                _lastRefresh = DateTime.UtcNow;
                _logger.LogInformation("Frozen snowflake emoji ID loaded from config: {EmojiId}", configuredId.Value);
                return;
            }

            // If not in config, try MongoDB
            var database = _mongoDatabase;
            var settingsCollection = database.GetCollection<BsonDocument>("settings");
            
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "frozen-account-settings");
            var settings = await settingsCollection.Find(filter).FirstOrDefaultAsync();

            if (settings != null && settings.Contains("snowflakeEmojiId"))
            {
                var emojiId = settings["snowflakeEmojiId"];
                if (emojiId.IsInt64)
                {
                    _cachedSnowflakeEmojiId = emojiId.AsInt64;
                    _lastRefresh = DateTime.UtcNow;
                    _logger.LogInformation("Frozen snowflake emoji ID loaded from MongoDB: {EmojiId}", _cachedSnowflakeEmojiId.Value);
                }
                else if (emojiId.IsInt32)
                {
                    _cachedSnowflakeEmojiId = emojiId.AsInt32;
                    _lastRefresh = DateTime.UtcNow;
                    _logger.LogInformation("Frozen snowflake emoji ID loaded from MongoDB: {EmojiId}", _cachedSnowflakeEmojiId.Value);
                }
                else if (emojiId.IsString)
                {
                    if (long.TryParse(emojiId.AsString, out var parsed))
                    {
                        _cachedSnowflakeEmojiId = parsed;
                        _lastRefresh = DateTime.UtcNow;
                        _logger.LogInformation("Frozen snowflake emoji ID loaded from MongoDB (parsed): {EmojiId}", _cachedSnowflakeEmojiId.Value);
                    }
                }
            }
            else
            {
                _logger.LogWarning("Frozen snowflake emoji ID not configured in appsettings.json or MongoDB");
                _cachedSnowflakeEmojiId = null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load frozen snowflake emoji ID from MongoDB");
            _cachedSnowflakeEmojiId = null;
        }
    }
}
