using System.Collections.Concurrent;
using System.Reflection;
using EventFlow.Extensions;
using EventFlow.MongoDB.ReadStores.Attributes;
using EventFlow.MongoDB.ValueObjects;
using EventFlow.ReadStores;

namespace MyTelegram.EventFlow.MongoDB.ReadStores;

public class QueryOnlyReadModelDescriptionProvider : IQueryOnlyReadModelDescriptionProvider
{
    private static readonly ConcurrentDictionary<Type, ReadModelDescription> CollectionNames = new();

    public ReadModelDescription GetReadModelDescription<TReadModel>() where TReadModel : IReadModel
    {
        var name = typeof(TReadModel).PrettyPrint().ToLowerInvariant();
        if (typeof(TReadModel).IsInterface && name.StartsWith("i"))
        {
            name = name[1..];
        }

        return CollectionNames.GetOrAdd(
            typeof(TReadModel),
            t =>
            {
                var collectionType = t.GetTypeInfo().GetCustomAttribute<MongoDbCollectionNameAttribute>();
                var indexName = collectionType == null
                    ? $"eventflow-{name}"
                    : collectionType.CollectionName;
                return new ReadModelDescription(new RootCollectionName(indexName));
            });
    }
}