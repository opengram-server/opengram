namespace MyTelegram.QueryHandlers.MongoDB.UserName;

public class
    GetUserNameListByNamesQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<GetUserNameListByNamesQuery,
        IReadOnlyCollection<IUserNameReadModel>>
{
    public async Task<IReadOnlyCollection<IUserNameReadModel>> ExecuteQueryAsync(GetUserNameListByNamesQuery query, CancellationToken cancellationToken)
    {
        Expression<Func<UserNameReadModel, bool>> predicate = p => query.UserNames.Contains(p.UserName);
        if (query.PeerType.HasValue)
        {
            predicate = predicate.And(p => p.PeerType == query.PeerType.Value);
        }
        return await store.FindAsync(predicate, cancellationToken: cancellationToken);
    }
}