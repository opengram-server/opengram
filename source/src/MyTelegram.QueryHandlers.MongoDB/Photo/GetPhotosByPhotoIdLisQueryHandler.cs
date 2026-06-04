namespace MyTelegram.QueryHandlers.MongoDB.Photo;

public class
    GetPhotosByPhotoIdLisQueryHandler(IQueryOnlyReadModelStore<PhotoReadModel> store)
    : IQueryHandler<GetPhotosByPhotoIdLisQuery, IReadOnlyCollection<IPhotoReadModel>>
{
    public async Task<IReadOnlyCollection<IPhotoReadModel>> ExecuteQueryAsync(GetPhotosByPhotoIdLisQuery query,
        CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => query.PhotoIds.Contains(p.PhotoId), cancellationToken: cancellationToken);
    }
}