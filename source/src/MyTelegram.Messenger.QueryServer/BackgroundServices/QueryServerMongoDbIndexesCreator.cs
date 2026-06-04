using EventFlow.MongoDB.EventStore;
using EventFlow.MongoDB.ReadStores;
using MongoDB.Driver;

namespace MyTelegram.Messenger.QueryServer.BackgroundServices;

public class QueryServerMongoDbIndexesCreator(
    IMongoDatabase database,
    IReadModelDescriptionProvider descriptionProvider,
    IMongoDbEventPersistenceInitializer eventPersistenceInitializer)
    : MongoDbIndexesCreatorBase(database, descriptionProvider, eventPersistenceInitializer), ITransientDependency
{
    protected override async Task CreateAllIndexesCoreAsync()
    {
        await CreateIndexAsync<RpcResultReadModel>(p => p.UserId);
        await CreateIndexAsync<RpcResultReadModel>(p => p.ReqMsgId);

        await CreateIndexAsync<UpdatesReadModel>(p => p.OwnerPeerId);
        await CreateIndexAsync<UpdatesReadModel>(p => p.ChannelId);
        await CreateIndexAsync<UpdatesReadModel>(p => p.Pts);

        await CreateIndexAsync<PtsReadModel>(p => p.PeerId);
    }
}