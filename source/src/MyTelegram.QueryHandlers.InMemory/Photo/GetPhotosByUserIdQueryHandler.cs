namespace MyTelegram.QueryHandlers.InMemory.Photo;

public class GetPhotosByUserIdQueryHandler(IQueryOnlyReadModelStore<PhotoReadModel> store) : IQueryHandler<GetPhotosByUserIdQuery, IReadOnlyCollection<IPhotoReadModel>>
{
    public async Task<IReadOnlyCollection<IPhotoReadModel>> ExecuteQueryAsync(GetPhotosByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await store.FindAsync(p => p.UserId == query.UserId && query.PhotoIds.Contains(p.PhotoId), cancellationToken: cancellationToken);
    }
}