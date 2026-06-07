using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Queries.Stars;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.Domain.Aggregates.Stars;
using DomainStarsTransaction = MyTelegram.Domain.Shared.Stars.StarsTransaction;

namespace MyTelegram.QueryHandlers.MongoDB.Stars;

public class GetStarsStatusQueryHandler(IQueryOnlyReadModelStore<StarsReadModel> store) : IQueryHandler<GetStarsStatusQuery, StarsStatus?>
{
    public async Task<StarsStatus?> ExecuteQueryAsync(GetStarsStatusQuery query, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[GetStarsStatusQueryHandler] Querying balance for PeerId: {query.PeerId} (type: {query.PeerId.GetType().Name})");

        // Первая попытка: ищем по PeerId
        Console.WriteLine($"Attempting query with PeerId == {query.PeerId}");
        var readModel = await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId, cancellationToken);

        if (readModel == null)
        {
            Console.WriteLine($"[GetStarsStatusQueryHandler] No StarsReadModel found for PeerId: {query.PeerId}");

            // Вторая попытка: ищем по сгенерированному StarsId
            var starsId = StarsId.Create(query.PeerId).Value;
            Console.WriteLine($"Trying alternative query by Id: {starsId}");
            readModel = await store.FirstOrDefaultAsync(p => p.Id == starsId, cancellationToken);

            if (readModel == null)
            {
                Console.WriteLine($"[GetStarsStatusQueryHandler] Also not found by Id: {starsId}. Returning default zero status.");
                return new StarsStatus
                {
                    Balance = 0,
                    History = []
                };
            }
            else
            {
                Console.WriteLine($"[GetStarsStatusQueryHandler] Found by Id! PeerId: {readModel.PeerId}, Balance: {readModel.Balance}");
            }
        }

        Console.WriteLine($"[GetStarsStatusQueryHandler] Retrieved stars status: balance {readModel.Balance}, transactions {readModel.Transactions.Count} for peer {query.PeerId}");
        return new StarsStatus
        {
            Balance = readModel.Balance,
            History = readModel.Transactions.Select(x => new DomainStarsTransaction
            {
                Id = x.Id,
                Amount = x.Amount,
                Date = x.Date,
                Title = x.Reason,
                PeerId = query.PeerId
            }).ToList()
        };
    }
}
