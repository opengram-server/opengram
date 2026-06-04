using MongoDB.Driver;
using MongoDB.Driver.Linq;
using MyTelegram.Queries.ScheduledMessage;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.Domain.Aggregates.Messaging;

namespace MyTelegram.QueryHandlers.MongoDB.ScheduledMessage;

public class GetScheduledMessagesQueryHandler(IMongoDatabase database) 
    : IQueryHandler<GetScheduledMessagesQuery, IReadOnlyList<IScheduledMessageReadModel>>
{
    public async Task<IReadOnlyList<IScheduledMessageReadModel>> ExecuteQueryAsync(
        GetScheduledMessagesQuery query, 
        CancellationToken cancellationToken)
    {
        var collection = database.GetCollection<ScheduledMessageReadModel>("ReadModel-ScheduledMessageReadModel");
        
        var ids = query.ScheduleMessageIds
            .Select(id => ScheduledMessageId.Create(query.OwnerPeerId, id))
            .ToList();
        
        var messages = await collection
            .AsQueryable()
            .Where(m => ids.Contains(m.Id) && !m.IsSent)
            .OrderByDescending(m => m.ScheduleDate)
            .ToListAsync(cancellationToken);
        
        return messages;
    }
}
