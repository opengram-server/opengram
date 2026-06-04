using MongoDB.Driver;

namespace MyTelegram.Messenger.Services.Impl;

public class FrozenIconService : IFrozenIconService
{
    private readonly IMongoDatabase _database;
    private string? _cachedIcon;
    private DateTime _lastCacheTime = DateTime.MinValue;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public FrozenIconService(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<string> GetFrozenIconAsync()
    {
        // Check cache
        if (_cachedIcon != null && DateTime.UtcNow - _lastCacheTime < _cacheExpiration)
        {
            return _cachedIcon;
        }

        try
        {
            var collection = _database.GetCollection<MongoDB.Bson.BsonDocument>("admin-settings");
            var filter = MongoDB.Bson.BsonDocument.Parse("{\"key\": \"frozen_icon\"}");
            var settings = await collection.Find(filter).FirstOrDefaultAsync();

            if (settings != null)
            {
                var type = settings.GetValue("type", "emoji").AsString;
                
                if (type == "emoji")
                {
                    _cachedIcon = settings.GetValue("emoji", "❄️").AsString;
                }
                else if (type == "animation")
                {
                    // For animation, still use emoji as fallback in name
                    // Animation URL can be used in client for rendering
                    _cachedIcon = settings.GetValue("emoji", "❄️").AsString;
                }
                else
                {
                    _cachedIcon = "❄️"; // Default
                }
            }
            else
            {
                _cachedIcon = "❄️"; // Default if no settings
            }

            _lastCacheTime = DateTime.UtcNow;
        }
        catch
        {
            _cachedIcon = "❄️"; // Fallback on error
        }

        return _cachedIcon;
    }
}
