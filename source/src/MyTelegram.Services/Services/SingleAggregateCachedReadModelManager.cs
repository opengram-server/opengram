using EventFlow.ReadStores;

namespace MyTelegram.Services.Services;

public class SingleAggregateCachedReadModelManager<TReadModelInterface, TReadModel>(
    IReadModelDomainEventApplier readModelDomainEventApplier,
    IServiceProvider serviceProvider,
    IReadModelCacheHelper<TReadModelInterface> readModelCacheHelper) :
    CachedReadModelManager<TReadModelInterface, TReadModel>(readModelDomainEventApplier, serviceProvider,
        readModelCacheHelper) where TReadModel : class, IReadModel where TReadModelInterface : IReadModel
{
    protected override IEnumerable<string> GetReadModelIds(IDomainEvent domainEvent)
    {
        yield return domainEvent.GetIdentity().Value;
    }
}