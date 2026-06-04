using MyTelegram.ReadModel;

namespace MyTelegram.QueryHandlers.MongoDB.UserPassword;

public class GetUserPasswordQueryHandler(IQueryOnlyReadModelStore<UserPasswordReadModel> store) 
    : IQueryHandler<GetUserPasswordQuery, IUserPasswordReadModel?>
{
    public async Task<IUserPasswordReadModel?> ExecuteQueryAsync(GetUserPasswordQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId == query.UserId, cancellationToken);
    }
}
