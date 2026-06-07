using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Bot;

public class GetBotsByOwnerUserIdQueryHandler(
    IQueryOnlyReadModelStore<BotReadModel> store,
    ILogger<GetBotsByOwnerUserIdQueryHandler> logger)
    : IQueryHandler<GetBotsByOwnerUserIdQuery, IReadOnlyCollection<IBotReadModel>>
{
    public async Task<IReadOnlyCollection<IBotReadModel>> ExecuteQueryAsync(
        GetBotsByOwnerUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var bots = await store.FindAsync(
            x => x.OwnerUserId == query.OwnerUserId,
            cancellationToken: cancellationToken);

        return bots.ToList();
    }
}
