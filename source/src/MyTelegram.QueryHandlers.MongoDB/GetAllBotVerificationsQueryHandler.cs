using MyTelegram.Queries;
using MyTelegram.ReadModel;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB;

public class GetAllBotVerificationsQueryHandler(IQueryOnlyReadModelStore<BotVerificationReadModel> store) 
    : IQueryHandler<GetAllBotVerificationsQuery, IReadOnlyCollection<IBotVerificationReadModel>>
{
    public async Task<IReadOnlyCollection<IBotVerificationReadModel>> ExecuteQueryAsync(
        GetAllBotVerificationsQuery query,
        CancellationToken cancellationToken)
    {
        // Get all bot verifications from MongoDB
        var verifications = await store.FindAsync(p => true);
        return verifications.ToList();
    }
}
