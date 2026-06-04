using EventFlow.MongoDB.ReadStores;
using EventFlow.Queries;
using MongoDB.Driver;
using MyTelegram.ReadModel;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.EventFlow.MongoDB;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetAvailableEffectsQueryHandler : IQueryHandler<GetAvailableEffectsQuery, IReadOnlyCollection<IMessageEffectReadModel>>
{
    private readonly IMyMongoDbReadModelStore<ReactionReadModel> _readModelStore;

    public GetAvailableEffectsQueryHandler(IMyMongoDbReadModelStore<ReactionReadModel> readModelStore)
    {
        _readModelStore = readModelStore;
    }

    public async Task<IReadOnlyCollection<IMessageEffectReadModel>> ExecuteQueryAsync(GetAvailableEffectsQuery query, CancellationToken cancellationToken)
    {
        var cursor = await _readModelStore.FindAsync(p => p.EffectAnimation != null, cancellationToken: cancellationToken);
        var list = await cursor.ToListAsync(cancellationToken);
        
        return list.Select(p => new MyTelegram.ReadModel.Impl.MessageEffectReadModel(
            p.EffectAnimation!.Value,
            p.Emoji,
            null,
            p.EffectAnimation.Value,
            p.EffectAnimation,
            p.Premium
        )).ToList();
    }
}
