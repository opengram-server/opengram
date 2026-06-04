namespace MyTelegram.QueryHandlers.MongoDB.Photo;

public class
    GetPhotoListQueryHandler(IQueryOnlyReadModelStore<PhotoReadModel> store)
    : IQueryHandler<GetPhotoListQuery, IReadOnlyCollection<IPhotoReadModel>>
{
    public async Task<IReadOnlyCollection<IPhotoReadModel>> ExecuteQueryAsync(GetPhotoListQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.UserId && query.PhotoIds.Contains(p.PhotoId), cancellationToken: cancellationToken);
    }
}