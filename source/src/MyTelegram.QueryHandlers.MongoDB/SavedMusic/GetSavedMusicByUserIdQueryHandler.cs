using MyTelegram.ReadModel;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.SavedMusic;

public class GetSavedMusicByUserIdQueryHandler(IMongoDatabase database)
    : IQueryHandler<GetSavedMusicByUserIdQuery, ISavedMusicReadModel?>
{
    public async Task<ISavedMusicReadModel?> ExecuteQueryAsync(
        GetSavedMusicByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<SavedMusicReadModel>("ReadModel-SavedMusicReadModel");
        
        return await collection
            .Find(x => x.UserId == query.UserId)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
