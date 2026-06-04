using EventFlow.MongoDB.EventStore;
using MyTelegram.Services.Services.IdGenerator;

namespace MyTelegram.Messenger.Services.Impl;

public class MongoDbHighValueGenerator(IMongoDbEventSequenceStore eventSequenceStore) : IHiLoHighValueGenerator, ITransientDependency
{
    public Task<long> GetNewHighValueAsync(IdType idType, long key, CancellationToken cancellationToken = default)
    {
        var nextId=eventSequenceStore.GetNextSequence($"{idType}-{key}");
        return Task.FromResult(nextId);
    }
}
