using MyTelegram.ReadModel.Impl;
using MyTelegram.Queries.StarGift;

namespace MyTelegram.QueryHandlers.MongoDB.StarGift;

public class GetStarGiftByMessageIdQueryHandler(IQueryOnlyReadModelStore<StarGiftReadModel> store) 
    : IQueryHandler<GetStarGiftByMessageIdQuery, IStarGiftReadModel?>
{
    public async Task<IStarGiftReadModel?> ExecuteQueryAsync(GetStarGiftByMessageIdQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[GetStarGiftByMessageIdQueryHandler] Searching for gift: UserId={query.UserId}, MessageId={query.MessageId}");

        // Ищем подарок по:
        // 1. MessageId входящего сообщения (получатель смотрит входящие)
        // 2. MessageId исходящего сообщения (отправитель смотрит исходящие или получатель смотрит в чате)
        // 3. SavedId (когда подарок сохранён в «Избранное»)
        // По UpgradeMsgId намеренно не ищем, иначе нашлись бы уже улучшенные подарки.
        // Сначала отдаём предпочтение НЕулучшенным подаркам, чтобы их можно было улучшить.
        var allMatches = await store.FindAsync(
            x => (x.ToUserId == query.UserId && x.MessageId == query.MessageId && (query.PeerId == null || x.ToPeerId == query.PeerId)) ||
                 (x.ToUserId == query.UserId && x.OutboxMessageId == query.MessageId && (query.PeerId == null || x.ToPeerId == query.PeerId)) || // Получатель видит OutboxMessageId в чате
                 (x.FromUserId == query.UserId && x.OutboxMessageId == query.MessageId && (query.PeerId == null || x.ToPeerId == query.PeerId)) ||
                 (x.ToUserId == query.UserId && x.SavedId == (long)query.MessageId), 
            skip: 0,
            limit: 100,
            sort: null,
            cancellationToken: cancellationToken);
        
        IStarGiftReadModel? result = null;
        if (allMatches?.Count > 0)
        {
            // Выбираем подарок в зависимости от флага preferUpgraded
            if (query.PreferUpgraded)
            {
                // Для перепродажи предпочитаем улучшенные подарки
                result = allMatches.FirstOrDefault(x => x.Upgraded) ?? allMatches.First();
            }
            else
            {
                // Для улучшения предпочитаем НЕулучшенные подарки, чтобы пользователь мог их улучшить
                result = allMatches.FirstOrDefault(x => !x.Upgraded) ?? allMatches.First();
            }

            if (allMatches.Count > 1)
            {
                var selectedStatus = result.Upgraded ? "upgraded" : "NOT upgraded";
                var reason = query.PreferUpgraded ? "(preferUpgraded=true)" : "(can be upgraded)";
                Console.WriteLine($"[GetStarGiftByMessageIdQueryHandler] Found {allMatches.Count} gifts with MessageId={query.MessageId}, selected {selectedStatus} {reason}");
            }
        }

        if (result == null)
        {
            Console.WriteLine($"[GetStarGiftByMessageIdQueryHandler] Gift not found for UserId={query.UserId}, MessageId={query.MessageId}");
            Console.WriteLine($"   Searched by: MessageId, OutboxMessageId, SavedId (NOT by UpgradeMsgId to avoid finding upgraded gifts)");
        }
        else
        {
            Console.WriteLine($"[GetStarGiftByMessageIdQueryHandler] Found gift: Id={result.Id}, GiftId={result.GiftId}, MessageId={result.MessageId}, Upgraded={result.Upgraded}, SavedId={result.SavedId}");
        }
        
        return result;
    }
}
