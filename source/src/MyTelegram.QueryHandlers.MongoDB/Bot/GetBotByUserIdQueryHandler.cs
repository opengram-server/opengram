using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB.Bot;

public class GetBotByUserIdQueryHandler(
    IQueryOnlyReadModelStore<BotReadModel> store,
    ILogger<GetBotByUserIdQueryHandler> logger)
    : IQueryHandler<GetBotByUserIdQuery, IBotReadModel?>
{
    public async Task<IBotReadModel?> ExecuteQueryAsync(
        GetBotByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var bots = await store.FindAsync(
            x => x.UserId == query.UserId,
            cancellationToken: cancellationToken);

        return bots.FirstOrDefault();
    }
}
