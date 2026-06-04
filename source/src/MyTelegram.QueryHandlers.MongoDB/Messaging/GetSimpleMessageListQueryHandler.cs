namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class
    GetSimpleMessageListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetSimpleMessageListQuery, IReadOnlyCollection<SimpleMessageItem>>
{
    public async Task<IReadOnlyCollection<SimpleMessageItem>> ExecuteQueryAsync(GetSimpleMessageListQuery query, CancellationToken cancellationToken)
    {
        if (query.ToPeer.PeerType == PeerType.Channel)
        {
            Expression<Func<MessageReadModel, bool>> getChannelMessagesPredicate = p => p.OwnerPeerId == query.ToPeer.PeerId;
            getChannelMessagesPredicate = getChannelMessagesPredicate.WhereIf(query.Pinned.HasValue, p => p.Pinned == query.Pinned!.Value)
                    .WhereIf(query.MessageIds?.Count > 0, p => query.MessageIds!.Contains(p.MessageId))
                ;

            return await store.FindAsync(getChannelMessagesPredicate,
                p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId), limit: query.Limit,
                cancellationToken: cancellationToken);
        }

        Expression<Func<MessageReadModel, bool>> predicate = p => p.OwnerPeerId == query.OwnerPeerId && p.ToPeerId == query.ToPeer.PeerId;
        predicate = predicate
                .WhereIf(query.Pinned.HasValue, p => p.Pinned == query.Pinned!.Value)
                .WhereIf(query.MessageIds?.Count > 0, p => query.MessageIds!.Contains(p.MessageId))
            ;

        if (!query.IncludeOtherParticipantMessages)
        {
            return await store.FindAsync(predicate,
                p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
                limit: query.Limit, cancellationToken: cancellationToken
            );
        }

        var batchIds = await store.FindAsync(
            predicate,
            p => p.BatchId,
            limit: query.Limit, cancellationToken: cancellationToken);

        if (batchIds.Count == 0)
        {
            return [];
        }

        //predicate = predicate.And(p => batchIds.Contains(p.BatchId));

        return await store.FindAsync(p => batchIds.Contains(p.BatchId),
            p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
            cancellationToken: cancellationToken);
    }
}