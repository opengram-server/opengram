using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTelegram.Queries.ScheduledMessage;
using MyTelegram.ReadModel.MongoDB;

namespace MyTelegram.QueryHandlers.MongoDB.ScheduledMessage;

public class GetPendingScheduledMessagesQueryHandler(IMongoDatabase database) 
    : IQueryHandler<GetPendingScheduledMessagesQuery, IReadOnlyList<IScheduledMessageReadModel>>
{
    public async Task<IReadOnlyList<IScheduledMessageReadModel>> ExecuteQueryAsync(
        GetPendingScheduledMessagesQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<ScheduledMessageReadModel>("ReadModel-ScheduledMessageReadModel");
        
        // Find messages that:
        // 1. Are not sent yet
        // 2. Schedule date is less than or equal to current time
        var messages = await collection
            .AsQueryable()
            .Where(m => !m.IsSent && m.ScheduleDate <= query.CurrentUnixTime)
            .OrderBy(m => m.ScheduleDate)
            .Take(query.Limit)
            .ToListAsync(cancellationToken);
        
        return messages;
    }
}
