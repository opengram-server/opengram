namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetUsersByIdListQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) 
    : IQueryHandler<GetUsersByIdListQuery, IReadOnlyCollection<IUserReadModel>>
{
    public async Task<IReadOnlyCollection<IUserReadModel>> ExecuteQueryAsync(GetUsersByIdListQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => query.UserIdList.Contains(p.UserId), cancellationToken: cancellationToken);
    }
}
