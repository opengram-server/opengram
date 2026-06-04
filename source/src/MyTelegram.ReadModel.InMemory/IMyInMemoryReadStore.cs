using MyTelegram.EventFlow.ReadStores;

namespace MyTelegram.ReadModel.InMemory;

public interface IMyInMemoryReadStore<TReadModel> : IInMemoryReadStore<TReadModel>,
    IQueryOnlyReadModelStore<TReadModel>
    where TReadModel : class, IReadModel
{
    Task<IQueryable<TReadModel>> AsQueryable(CancellationToken cancellationToken = default);

    //void Add(string id, TReadModel readModel, long? version);
}