using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetStarGiftBySavedIdQueryHandler(IQueryOnlyReadModelStore<StarGiftReadModel> store) 
    : IQueryHandler<GetStarGiftBySavedIdQuery, IStarGiftReadModel?>
{
    public async Task<IStarGiftReadModel?> ExecuteQueryAsync(GetStarGiftBySavedIdQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[GetStarGiftBySavedIdQueryHandler] Searching for gift: UserId={query.UserId}, SavedId={query.SavedId}");

        var result = await store.FirstOrDefaultAsync(
            x => x.ToUserId == query.UserId && x.SavedId == query.SavedId,
            cancellationToken);

        if (result == null)
        {
            Console.WriteLine($"[GetStarGiftBySavedIdQueryHandler] Gift not found for UserId={query.UserId}, SavedId={query.SavedId}");
        }
        else
        {
            Console.WriteLine($"[GetStarGiftBySavedIdQueryHandler] Found gift: Id={result.Id}, GiftId={result.GiftId}, Upgraded={result.Upgraded}");
        }
        
        return result;
    }
}
