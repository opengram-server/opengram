using MyTelegram.Queries;
using MyTelegram.ReadModel;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetBotVerifierByBotIdQueryHandler(IQueryOnlyReadModelStore<BotVerifierReadModel> store) 
    : IQueryHandler<GetBotVerifierByBotIdQuery, IBotVerifierReadModel?>
{
    public async Task<IBotVerifierReadModel?> ExecuteQueryAsync(
        GetBotVerifierByBotIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.BotUserId == query.BotUserId && p.IsActive, 
            cancellationToken);
    }
}
