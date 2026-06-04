namespace MyTelegram.QueryHandlers.MongoDB.User;

public class
    GetUsersByUserIdListQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetUsersByUserIdListQuery, IReadOnlyCollection<IUserReadModel>>
{
    public async Task<IReadOnlyCollection<IUserReadModel>> ExecuteQueryAsync(GetUsersByUserIdListQuery query,
        CancellationToken cancellationToken)
    {
        if (query.UserIdList.Count == 0)
        {
            return new List<UserReadModel>();
        }

        return await store.FindAsync(p => query.UserIdList.Contains(p.UserId), cancellationToken: cancellationToken);
    }
}
