using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MyTelegram.Queries;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.ScheduledMessage;

public class GetScheduleMessagesByDateQueryHandler(
    IMongoDatabase database,
    ILogger<GetScheduleMessagesByDateQueryHandler> logger) : 
    IQueryHandler<GetScheduleMessagesByDateQuery, IReadOnlyCollection<ScheduleItem>>
{
    public async Task<IReadOnlyCollection<ScheduleItem>> ExecuteQueryAsync(
        GetScheduleMessagesByDateQuery query,
        CancellationToken cancellationToken)
    {
        logger.LogWarning(
            "*** Querying scheduled messages: MaxScheduleDate={MaxScheduleDate} ***",
            query.MaxScheduleDate);
        
        // Get collection directly from MongoDB - use EventFlow naming convention
        var collection = database.GetCollection<MyTelegram.ReadModel.Impl.ScheduledMessageReadModel>("eventflow-scheduledmessagereadmodel");
        
        // Log all scheduled messages in collection for debugging
        try
        {
            var allMessages = await collection.Find(Builders<MyTelegram.ReadModel.Impl.ScheduledMessageReadModel>.Filter.Empty)
                .Limit(10)
                .ToListAsync(cancellationToken);
            
            logger.LogWarning(
                "*** Total scheduled messages in collection (first 10): {Count} ***",
                allMessages.Count);
            
            foreach (var msg in allMessages)
            {
                logger.LogWarning(
                    "*** Scheduled message: Id={Id}, ScheduleDate={ScheduleDate}, IsSent={IsSent}, UserId={UserId} ***",
                    msg.Id,
                    msg.ScheduleDate,
                    msg.IsSent,
                    msg.SenderUserId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "*** Error reading all scheduled messages ***");
        }
        
        // Find all scheduled messages where ScheduleDate <= MaxScheduleDate (current time) AND not yet sent
        var filterBuilder = Builders<MyTelegram.ReadModel.Impl.ScheduledMessageReadModel>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Lte(x => x.ScheduleDate, query.MaxScheduleDate),
            filterBuilder.Eq(x => x.IsSent, false)
        );
        
        logger.LogWarning(
            "*** Query filter: ScheduleDate <= {MaxScheduleDate} AND IsSent = false ***",
            query.MaxScheduleDate);
        
        var messages = await collection.Find(filter).ToListAsync(cancellationToken);
        
        logger.LogWarning(
            "*** Found {Count} scheduled messages matching filter ***",
            messages.Count);

        return messages
            .Select(m => new ScheduleItem(
                m.SenderUserId,
                m.ToPeer,
                m.ScheduleMessageId,
                m.ScheduleDate,
                m.GroupedId))
            .ToList();
    }
}
