using MongoDB.Driver;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Contacts;

///<summary>
/// Generates a temporary profile link for sharing.
/// See <a href="https://corefork.telegram.org/method/contacts.exportContactToken" />
///</summary>
internal sealed class ExportContactTokenHandler(
    IRandomHelper randomHelper,
    IMongoDatabase mongoDatabase,
    ILogger<ExportContactTokenHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Contacts.RequestExportContactToken, MyTelegram.Schema.IExportedContactToken>,
    Contacts.IExportContactTokenHandler
{
    protected override async Task<IExportedContactToken> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Contacts.RequestExportContactToken obj)
    {
        // Generate a cryptographically random token
        var tokenBytes = randomHelper.NextBytes(16);
        var token = Convert.ToHexString(tokenBytes).ToLowerInvariant();
        var expires = CurrentDate + 86400; // Valid for 24 hours

        // Persist the token so ImportContactToken can resolve it
        var collection = mongoDatabase.GetCollection<ContactTokenDocument>("ContactTokens");

        // Upsert: replace any existing token for this user
        var filter = Builders<ContactTokenDocument>.Filter.Eq(d => d.UserId, input.UserId);
        var replacement = new ContactTokenDocument
        {
            UserId = input.UserId,
            Token = token,
            ExpiresAt = DateTimeOffset.FromUnixTimeSeconds(expires).UtcDateTime
        };

        await collection.ReplaceOneAsync(filter, replacement, new ReplaceOptions { IsUpsert = true });

        // Ensure TTL index for automatic cleanup of expired tokens
        var indexKeys = Builders<ContactTokenDocument>.IndexKeys.Ascending(d => d.ExpiresAt);
        var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.Zero };
        try
        {
            await collection.Indexes.CreateOneAsync(
                new CreateIndexModel<ContactTokenDocument>(indexKeys, indexOptions));
        }
        catch
        {
            // Index may already exist
        }

        logger.LogDebug("ExportContactToken: Generated token for UserId={UserId}, expires={Expires}",
            input.UserId, expires);

        return new TExportedContactToken
        {
            Url = $"https://t.me/contact/{token}",
            Expires = expires
        };
    }
}

/// <summary>
/// MongoDB document for storing contact sharing tokens.
/// </summary>
internal class ContactTokenDocument
{
    public long UserId { get; set; }
    public string Token { get; set; } = default!;
    public DateTime ExpiresAt { get; set; }
}
