namespace MyTelegram.QueryHandlers.MongoDB.User;

public class GetUserProfilePhotoIdQueryHandler(IQueryOnlyReadModelStore<UserReadModel> store) : IQueryHandler<GetUserProfilePhotoIdQuery, long?>
{
    public async Task<long?> ExecuteQueryAsync(GetUserProfilePhotoIdQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.UserId == query.UserId, p => p.ProfilePhotoId, cancellationToken: cancellationToken);
    }
}