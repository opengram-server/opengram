using EventFlow.Aggregates;
using EventFlow.Subscribers;
using MongoDB.Driver;
using MyTelegram.Domain.Aggregates.User;
using MyTelegram.Domain.Events.User;
using MyTelegram.Domain.Extensions;
using MyTelegram.EventFlow.MongoDB;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.Messenger.CommandServer.EventHandlers;

/// <summary>
/// Event handler that saves installed sticker sets to MongoDB when user installs a sticker set
/// </summary>
public class StickerSetInstalledEventHandler(
    DefaultReadModelMongoDbContext dbContext,
    ILogger<StickerSetInstalledEventHandler> logger)
    : ISubscribeSynchronousTo<UserAggregate, UserId, StickerSetInstalledEvent>
{
    public Task HandleAsync(IDomainEvent<UserAggregate, UserId, StickerSetInstalledEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        
        logger.LogWarning(
            "*** Saving installed sticker set: UserId={UserId}, StickerSetId={StickerSetId}, Type={Type} ***",
            evt.UserId,
            evt.StickerSetId,
            evt.StickerSetType);

        var database = dbContext.GetDatabase();
        var collection = database.GetCollection<InstalledStickerSetReadModel>("eventflow-installedstickersetreadmodel");
        
        var id = $"{evt.UserId}_{evt.StickerSetId}";
        
        var readModel = new InstalledStickerSetReadModel
        {
            Id = id,
            Version = 1,
            UserId = evt.UserId,
            StickerSetId = evt.StickerSetId,
            Archived = evt.Archived,
            StickerSetType = evt.StickerSetType,
            Date = DateTime.UtcNow.ToTimestamp()
        };

        // Use ReplaceOne with upsert to insert or update
        var filter = Builders<InstalledStickerSetReadModel>.Filter.Eq(x => x.Id, id);
        var options = new ReplaceOptions { IsUpsert = true };
        
        collection.ReplaceOne(filter, readModel, options, cancellationToken);
        
        logger.LogWarning("*** Installed sticker set saved to MongoDB: {Id} ***", id);
        
        return Task.CompletedTask;
    }
}
