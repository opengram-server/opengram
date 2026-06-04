using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetStarGiftByCollectibleIdQueryHandler(IQueryOnlyReadModelStore<StarGiftReadModel> store) 
    : IQueryHandler<GetStarGiftByCollectibleIdQuery, IStarGiftReadModel?>
{
    public async Task<IStarGiftReadModel?> ExecuteQueryAsync(GetStarGiftByCollectibleIdQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Searching for gift: UserId={query.UserId}, CollectibleId={query.CollectibleId}");

        // Сначала пробуем найти по сохранённому CollectibleId
        var result = await store.FirstOrDefaultAsync(
            x => x.ToUserId == query.UserId && x.CollectibleId == query.CollectibleId,
            cancellationToken);

        if (result == null)
        {
            Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Gift not found by stored CollectibleId, trying dynamic calculation...");

            // Если не нашли, пробуем вычислить CollectibleId на лету.
            // Это нужно для случаев, когда CollectibleId пуст или сохранён неверно.
            var upgradedGifts = await store.FindAsync(
                x => x.ToUserId == query.UserId && x.Upgraded == true,
                skip: 0,
                limit: 0, // 0 означает без ограничения
                sort: null,
                cancellationToken: cancellationToken);

            Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Found {upgradedGifts.Count()} upgraded gifts for user {query.UserId}");

            foreach (var gift in upgradedGifts)
            {
                if (!string.IsNullOrEmpty(gift.AggregateId))
                {
                    var uniqueIdHash = gift.AggregateId.GetHashCode();
                    var calculatedCollectibleId = Math.Abs((long)uniqueIdHash) + (gift.GiftId * 1000000);

                    Console.WriteLine($"Checking gift {gift.AggregateId}: calculated={calculatedCollectibleId}, looking for={query.CollectibleId}, match={calculatedCollectibleId == query.CollectibleId}");

                    if (calculatedCollectibleId == query.CollectibleId)
                    {
                        Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Found gift by dynamic calculation: Id={gift.Id}, GiftId={gift.GiftId}, AggregateId={gift.AggregateId}");
                        result = gift;
                        break;
                    }
                }
                else
                {
                    Console.WriteLine($"Gift {gift.Id} has no AggregateId, skipping");
                }
            }
        }
        else
        {
            Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Found gift by stored CollectibleId: Id={result.Id}, GiftId={result.GiftId}, Upgraded={result.Upgraded}");
        }

        if (result == null)
        {
            Console.WriteLine($"[GetStarGiftByCollectibleIdQueryHandler] Gift not found for UserId={query.UserId}, CollectibleId={query.CollectibleId}");
        }
        
        return result;
    }
}
