using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using MyTelegram.EventFlow.MongoDB;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.CommandServer.BackgroundServices;

/// <summary>
/// One-time migration to set DefaultHistoryTTL = 0 for existing users where it's null
/// </summary>
public class UserDefaultHistoryTTLMigrationBackgroundService(
    DefaultReadModelMongoDbContext dbContext,
    ILogger<UserDefaultHistoryTTLMigrationBackgroundService> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            logger.LogWarning("*** Starting DefaultHistoryTTL migration for existing users ***");
            
            var database = dbContext.GetDatabase();
            var collection = database.GetCollection<UserReadModel>("eventflow-userreadmodel");
            
            // Find all users where DefaultHistoryTTL is null
            var filter = Builders<UserReadModel>.Filter.Eq(x => x.DefaultHistoryTTL, null);
            var update = Builders<UserReadModel>.Update.Set(x => x.DefaultHistoryTTL, 0);
            
            var result = await collection.UpdateManyAsync(filter, update, cancellationToken: stoppingToken);
            
            logger.LogWarning(
                "*** DefaultHistoryTTL migration completed: {MatchedCount} users found, {ModifiedCount} users updated ***",
                result.MatchedCount,
                result.ModifiedCount);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "*** Error during DefaultHistoryTTL migration ***");
        }
    }
}
