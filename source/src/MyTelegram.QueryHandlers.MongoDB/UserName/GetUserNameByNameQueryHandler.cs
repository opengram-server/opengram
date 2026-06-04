namespace MyTelegram.QueryHandlers.MongoDB.UserName;

public class
    GetUserNameByNameQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<GetUserNameByNameQuery, IUserNameReadModel?>
{
    public async Task<IUserNameReadModel?> ExecuteQueryAsync(GetUserNameByNameQuery query,
        CancellationToken cancellationToken)
    {
        var lowerUserName = query.Name.ToLower();
        return await store.FirstOrDefaultAsync(p => p.UserName.ToLower() == lowerUserName, cancellationToken);
    }
}
