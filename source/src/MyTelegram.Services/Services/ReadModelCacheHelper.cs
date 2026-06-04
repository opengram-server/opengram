using System.Collections.Concurrent;

namespace MyTelegram.Services.Services;

public class ReadModelCacheHelper<TReadModel> : IReadModelCacheHelper<TReadModel>
{
    private static readonly ConcurrentDictionary<long, TReadModel> ReadModels = [];
    private static readonly ConcurrentDictionary<string, long> ReadModelIds = [];

    public async Task<TReadModel?> GetOrCreateAsync(long readModelId, Func<Task<TReadModel?>> createFactory, Func<TReadModel, string> createReadModelIdFunc)
    {
        if (ReadModels.TryGetValue(readModelId, out var readModel))
        {
            return readModel;
        }

        readModel = await createFactory();
        if (readModel == null)
        {
            return readModel;
        }

        ReadModels.TryAdd(readModelId, readModel!);

        var id = createReadModelIdFunc(readModel!);
        ReadModelIds.TryAdd(id, readModelId);

        return readModel!;
    }

    public bool TryGetReadModel(long readModelId, out TReadModel? readModel)
    {
        return ReadModels.TryGetValue(readModelId, out readModel);
    }

    public bool TryGetReadModel(string readModelId, out TReadModel? readModel)
    {
        if (ReadModelIds.TryGetValue(readModelId, out var id))
        {
            return ReadModels.TryGetValue(id, out readModel);
        }

        readModel = default;

        return false;
    }

    public void Add(long id, string readModelId, TReadModel readModel)
    {
        ReadModelIds.TryAdd(readModelId, id);
        ReadModels.TryAdd(id, readModel);
    }

    public void Remove(string readModelId)
    {
        if (ReadModelIds.TryRemove(readModelId, out var id))
        {
            ReadModels.TryRemove(id, out _);
        }
    }

    public void Remove(long readModelId)
    {
        ReadModels.TryRemove(readModelId, out _);
    }

    public TReadModel? Get(string readModelId)
    {
        if (ReadModelIds.TryGetValue(readModelId, out var id))
        {
            ReadModels.TryGetValue(id, out var readModel);
            return readModel;
        }

        return default;
    }
}