using EventFlow.ReadStores;

namespace MyTelegram.Services.Services;

public interface IReadModelWithCacheAppService<TReadModel>
    where TReadModel : IReadModel
{
    Task<TReadModel?> GetAsync(long? id);
    Task<TReadModel> GetAsync(long id);

    Task<IReadOnlyCollection<TReadModel>> GetListAsync(IEnumerable<long> ids);
}