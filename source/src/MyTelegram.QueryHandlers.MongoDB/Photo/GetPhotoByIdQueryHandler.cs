namespace MyTelegram.QueryHandlers.MongoDB.Photo;

public class GetPhotoByIdQueryHandler(IQueryOnlyReadModelStore<PhotoReadModel> store)
    : IQueryHandler<GetPhotoByIdQuery, IPhotoReadModel?>
{
    public async Task<IPhotoReadModel?> ExecuteQueryAsync(GetPhotoByIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FirstOrDefaultAsync(p => p.PhotoId == query.PhotoId, cancellationToken);
    }
}