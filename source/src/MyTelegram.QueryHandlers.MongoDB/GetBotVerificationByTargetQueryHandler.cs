using MyTelegram.Queries;
using MyTelegram.ReadModel;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetBotVerificationByTargetQueryHandler(IQueryOnlyReadModelStore<BotVerificationReadModel> store) 
    : IQueryHandler<GetBotVerificationByTargetQuery, IBotVerificationReadModel?>
{
    public async Task<IBotVerificationReadModel?> ExecuteQueryAsync(
        GetBotVerificationByTargetQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(
            p => p.TargetType == query.TargetType && p.TargetId == query.TargetId, 
            cancellationToken);
    }
}
