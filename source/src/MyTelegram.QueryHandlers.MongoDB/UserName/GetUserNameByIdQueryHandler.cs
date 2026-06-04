namespace MyTelegram.QueryHandlers.MongoDB.UserName;

public class GetUserNameByIdQueryHandler(IQueryOnlyReadModelStore<UserNameReadModel> store) : IQueryHandler<GetUserNameByIdQuery, IUserNameReadModel?>
{
    public async Task<IUserNameReadModel?> ExecuteQueryAsync(GetUserNameByIdQuery query,
        CancellationToken cancellationToken)
    {
        var lowerName = query.UserName.ToLower();
        return await store.FirstOrDefaultAsync(p => p.UserName.ToLower() == lowerName, cancellationToken);
    }
}
