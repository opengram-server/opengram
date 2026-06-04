using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace MyTelegram.EventFlow.MongoDB;

public class 
    DefaultReadModelMongoDbContext(IConfiguration configuration) : IMongoDbContext
{
    private IMongoDatabase? _database;

    public IMongoDatabase GetDatabase()
    {
        if (_database == null)
        {
            var connectionString = configuration.GetConnectionString(GetConnectionStringName());
            var databaseName = configuration.GetValue<string>(GetKeyOfDatabaseNameInConfiguration());
            
            // Configure MongoClient with proper read settings to avoid stale data
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            settings.ReadConcern = ReadConcern.Local; // Read latest data without waiting for replication
            settings.ReadPreference = ReadPreference.Primary; // Always read from primary node
            settings.RetryReads = true; // Retry failed reads
            
            var client = new MongoClient(settings);
            _database = client.GetDatabase(databaseName);
        }

        return _database;
    }

    protected virtual string GetConnectionStringName() => "Default";
    protected virtual string GetKeyOfDatabaseNameInConfiguration() => "App:ReadModelDatabaseName";
}