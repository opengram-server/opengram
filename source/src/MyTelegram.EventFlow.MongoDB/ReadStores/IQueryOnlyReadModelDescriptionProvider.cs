using EventFlow.MongoDB.ValueObjects;
using EventFlow.ReadStores;

namespace MyTelegram.EventFlow.MongoDB.ReadStores;

public interface IQueryOnlyReadModelDescriptionProvider
{
    ReadModelDescription GetReadModelDescription<TReadModel>()
        where TReadModel : IReadModel;
}