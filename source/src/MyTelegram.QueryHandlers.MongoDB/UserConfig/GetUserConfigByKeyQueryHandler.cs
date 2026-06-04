namespace MyTelegram.QueryHandlers.MongoDB.UserConfig;
public class GetUserConfigByKeyQueryHandler(IQueryOnlyReadModelStore<UserConfigReadModel> store) : IQueryHandler<GetUserConfigByKeyQuery, IUserConfigReadModel?>
{
    public async Task<IUserConfigReadModel?> ExecuteQueryAsync(GetUserConfigByKeyQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId == query.UserId && p.Key == query.Key, cancellationToken);
    }
}
