using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTelegram.Queries.ScheduledMessage;
using MyTelegram.ReadModel.MongoDB;

namespace MyTelegram.QueryHandlers.MongoDB.ScheduledMessage;

public class GetScheduledHistoryQueryHandler(IMongoDatabase database) 
    : IQueryHandler<GetScheduledHistoryQuery, IReadOnlyList<IScheduledMessageReadModel>>
{
    public async Task<IReadOnlyList<IScheduledMessageReadModel>> ExecuteQueryAsync(
        GetScheduledHistoryQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<ScheduledMessageReadModel>("ReadModel-ScheduledMessageReadModel");
        
        var queryable = collection
            .AsQueryable()
            .Where(m => m.OwnerPeerId == query.OwnerPeerId && !m.IsSent);
        
        if (query.OffsetDate > 0)
        {
            queryable = queryable.Where(m => m.ScheduleDate < query.OffsetDate);
        }
        
        var messages = await queryable
            .OrderByDescending(m => m.ScheduleDate)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);
        
        return messages;
    }
}
