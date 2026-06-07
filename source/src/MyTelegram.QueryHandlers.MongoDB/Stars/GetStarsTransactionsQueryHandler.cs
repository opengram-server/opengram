using MyTelegram.Domain.Shared.Stars;
using MyTelegram.Queries.Stars;
using MyTelegram.ReadModel.Impl;
using MyTelegram.ReadModel.Interfaces;
using DomainStarsTransaction = MyTelegram.Domain.Shared.Stars.StarsTransaction;

namespace MyTelegram.QueryHandlers.MongoDB.Stars;

public class GetStarsTransactionsQueryHandler(IQueryOnlyReadModelStore<StarsReadModel> store) : IQueryHandler<GetStarsTransactionsQuery, StarsStatus?>
{
    public async Task<StarsStatus?> ExecuteQueryAsync(GetStarsTransactionsQuery query, CancellationToken cancellationToken)
    {
        var readModel = await store.FirstOrDefaultAsync(p => p.PeerId == query.PeerId, cancellationToken);
        if (readModel == null)
        {
            return new StarsStatus
            {
                Balance = 0,
                History = new List<DomainStarsTransaction>(),
                NextOffset = null,
                Subscriptions = new List<StarsSubscription>(),
                SubscriptionsNextOffset = null,
                SubscriptionsMissingBalance = null
            };
        }

        var transactions = readModel.Transactions.AsEnumerable();

        if (query.IsInbound)
        {
            transactions = transactions.Where(x => x.Amount > 0);
        }
        if (query.IsOutbound)
        {
            transactions = transactions.Where(x => x.Amount < 0);
        }

        // TODO: Implement pagination using Offset and Limit

        var history = transactions.Select(x => new DomainStarsTransaction
        {
            Id = x.Id,
            Amount = x.Amount,
            Date = x.Date,
            Title = x.Reason,
            PeerId = query.PeerId
        }).ToList();

        return new StarsStatus
        {
            Balance = readModel.Balance,
            History = history
        };
    }
}
