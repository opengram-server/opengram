using MongoDB.Driver;

namespace MyTelegram.SessionServer.BackgroundServices;

/// <summary>
/// Creates MongoDB indexes on startup for the SessionServer's read models.
/// Reconstructed from the original binary's MongoDbIndexesCreator.
/// The original SessionServer uses EventFlow with MongoDB event store and
/// snapshot persistence — this ensures proper indexes exist.
/// </summary>
public sealed class MongoDbIndexesCreatorBackgroundService : BackgroundService
{
    private readonly IMongoDatabase _database;
    private readonly ILogger<MongoDbIndexesCreatorBackgroundService> _logger;

    public MongoDbIndexesCreatorBackgroundService(
        IMongoDatabase database,
        ILogger<MongoDbIndexesCreatorBackgroundService> logger)
    {
        _database = database;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Creating MongoDB indexes for SessionServer...");

        try
        {
            // AuthKey read model indexes
            await CreateAuthKeyIndexesAsync(stoppingToken).ConfigureAwait(false);

            // Device read model indexes
            await CreateDeviceIndexesAsync(stoppingToken).ConfigureAwait(false);

            _logger.LogInformation("MongoDB indexes created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create MongoDB indexes");
        }
    }

    private async Task CreateAuthKeyIndexesAsync(CancellationToken ct)
    {
        var collection = _database.GetCollection<AuthKeyReadModel>("authkey_readmodels");

        var indexModels = new[]
        {
            new CreateIndexModel<AuthKeyReadModel>(
                Builders<AuthKeyReadModel>.IndexKeys.Ascending(x => x.AuthKeyId),
                new CreateIndexOptions { Unique = true, Background = true }),
            new CreateIndexModel<AuthKeyReadModel>(
                Builders<AuthKeyReadModel>.IndexKeys.Ascending(x => x.PermAuthKeyId),
                new CreateIndexOptions { Background = true }),
            new CreateIndexModel<AuthKeyReadModel>(
                Builders<AuthKeyReadModel>.IndexKeys.Ascending(x => x.UserId),
                new CreateIndexOptions { Background = true }),
            new CreateIndexModel<AuthKeyReadModel>(
                Builders<AuthKeyReadModel>.IndexKeys.Ascending(x => x.TempAuthKeyId),
                new CreateIndexOptions { Background = true })
        };

        await collection.Indexes.CreateManyAsync(indexModels, ct).ConfigureAwait(false);
    }

    private async Task CreateDeviceIndexesAsync(CancellationToken ct)
    {
        var collection = _database.GetCollection<DeviceReadModel>("device_readmodels");

        var indexModels = new[]
        {
            new CreateIndexModel<DeviceReadModel>(
                Builders<DeviceReadModel>.IndexKeys
                    .Ascending(x => x.UserId)
                    .Ascending(x => x.AuthKeyId),
                new CreateIndexOptions { Unique = true, Background = true }),
            new CreateIndexModel<DeviceReadModel>(
                Builders<DeviceReadModel>.IndexKeys.Ascending(x => x.UserId),
                new CreateIndexOptions { Background = true })
        };

        await collection.Indexes.CreateManyAsync(indexModels, ct).ConfigureAwait(false);
    }
}

/// <summary>Read model for auth key persistence in MongoDB.</summary>
public sealed class AuthKeyReadModel
{
    public string Id { get; set; } = string.Empty;
    public long AuthKeyId { get; set; }
    public long TempAuthKeyId { get; set; }
    public long PermAuthKeyId { get; set; }
    public long UserId { get; set; }
    public long ServerSalt { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ExpireAt { get; set; }
}

/// <summary>Read model for device registration persistence in MongoDB.</summary>
public sealed class DeviceReadModel
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public long AuthKeyId { get; set; }
    public long SessionId { get; set; }
    public int ApiId { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string Platform { get; set; } = string.Empty;
    public DateTime RegisteredAt { get; set; }
}
