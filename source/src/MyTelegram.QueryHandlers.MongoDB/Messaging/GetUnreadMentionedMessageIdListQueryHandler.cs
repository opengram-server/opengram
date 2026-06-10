namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

/// <summary>
/// MongoDB handler for fetching unread mentioned message IDs.
/// Queries messages where the user is in MentionedUserIds, filtered by peer and offset.
/// </summary>
public class GetUnreadMentionedMessageIdListQueryHandler(
    IMongoDatabase database)
    : IQueryHandler<GetUnreadMentionedMessageIdListQuery, IReadOnlyCollection<int>>
{
    public async Task<IReadOnlyCollection<int>> ExecuteQueryAsync(
        GetUnreadMentionedMessageIdListQuery query,
        CancellationToken cancellationToken)
    {
        // MessageReadModel alias from GlobalUsings is MongoDB.MessageReadModel
        // which inherits from Impl.MessageReadModel (has MessageId property)
        var collection = database.GetCollection<MessageReadModel>("ReadModel-MessageReadModel");

        var filterBuilder = Builders<MessageReadModel>.Filter;
        var filter = filterBuilder.And(
            filterBuilder.Eq(x => x.OwnerPeerId, query.OwnerUserId),
            filterBuilder.Eq(x => x.ToPeerId, query.ToPeerId),
            filterBuilder.AnyEq(x => x.MentionedUserIds, query.OwnerUserId),
            filterBuilder.Ne(x => x.Out, true) // Only inbox messages
        );

        // Apply offset if provided
        if (query.Offset != null && query.Offset.MaxId > 0)
        {
            filter = filterBuilder.And(filter,
                filterBuilder.Lt(x => x.MessageId, query.Offset.MaxId));
        }

        var messageIds = await collection
            .Find(filter)
            .SortByDescending(x => x.MessageId)
            .Skip(query.Skip)
            .Limit(query.Limit)
            .Project(x => x.MessageId)
            .ToListAsync(cancellationToken);

        return messageIds;
    }
}
