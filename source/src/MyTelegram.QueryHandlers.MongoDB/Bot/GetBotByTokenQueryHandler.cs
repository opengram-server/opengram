using Microsoft.Extensions.Logging;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;
using System.Linq.Expressions;

namespace MyTelegram.QueryHandlers.MongoDB.Bot;

public class GetBotByTokenQueryHandler(
    IQueryOnlyReadModelStore<BotReadModel> store,
    ILogger<GetBotByTokenQueryHandler> logger)
    : IQueryHandler<GetBotByTokenQuery, IBotReadModel?>
{
    public async Task<IBotReadModel?> ExecuteQueryAsync(
        GetBotByTokenQuery query,
        CancellationToken cancellationToken)
    {
        var bots = await store.FindAsync(
            x => x.Token == query.Token,
            cancellationToken: cancellationToken);

        return bots.FirstOrDefault();
    }
}
