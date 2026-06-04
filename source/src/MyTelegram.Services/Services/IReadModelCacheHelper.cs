namespace MyTelegram.Services.Services;

public interface IReadModelCacheHelper<TReadModel>
{
    Task<TReadModel?> GetOrCreateAsync(long readModelId, Func<Task<TReadModel?>> createFactory, Func<TReadModel, string> createReadModelIdFunc);
    bool TryGetReadModel(long readModelId, out TReadModel? readModel);
    bool TryGetReadModel(string readModelId, out TReadModel? readModel);
    void Add(long id, string readModelId, TReadModel readModel);
    void Remove(string readModelId);
    void Remove(long readModelId);
    TReadModel? Get(string readModelId);
}