using EventFlow.MongoDB.EventStore;
using EventFlow.MongoDB.ReadStores;
using EventFlow.MongoDB.ValueObjects;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace MyTelegram.EventFlow.MongoDB;

public abstract class MongoDbIndexesCreatorBase(
    IMongoDatabase database,
    IReadModelDescriptionProvider descriptionProvider,
    IMongoDbEventPersistenceInitializer eventPersistenceInitializer)
    : IMongoDbIndexesCreator
{
    public async Task CreateAllIndexesAsync()
    {
        eventPersistenceInitializer.Initialize();
        var snapShotCollectionName = "snapShots";
        await CreateIndexAsync<MongoDbSnapshotDataModel>(p => p.AggregateId, snapShotCollectionName);
        await CreateIndexAsync<MongoDbSnapshotDataModel>(p => p.AggregateName, snapShotCollectionName);
        await CreateIndexAsync<MongoDbSnapshotDataModel>(p => p.AggregateSequenceNumber, snapShotCollectionName);

        await CreateAllIndexesCoreAsync();
    }

    protected abstract Task CreateAllIndexesCoreAsync();

    protected async Task CreateIndexAsync<TReadModel>(Expression<Func<TReadModel, object>> field)
        where TReadModel : IMongoDbReadModel
    {
        var indexDefine = Builders<TReadModel>.IndexKeys.Ascending(field);
        var collectionName = descriptionProvider.GetReadModelDescription<TReadModel>().RootCollectionName;
        await database.GetCollection<TReadModel>(collectionName.Value).Indexes
            .CreateOneAsync(new CreateIndexModel<TReadModel>(indexDefine));
    }

    protected async Task CreateIndexAsync<TSnapshot>(Expression<Func<TSnapshot, object>> field,
        string collectionName)
    {
        var indexDefine = Builders<TSnapshot>.IndexKeys.Ascending(field);
        await database.GetCollection<TSnapshot>(collectionName).Indexes
            .CreateOneAsync(new CreateIndexModel<TSnapshot>(indexDefine));
    }
}