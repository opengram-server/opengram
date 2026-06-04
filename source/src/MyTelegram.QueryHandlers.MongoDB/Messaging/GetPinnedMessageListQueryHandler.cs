namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

public class GetPinnedMessageListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) :
    IQueryHandler<GetPinnedMessageListQuery, IReadOnlyCollection<SimpleMessageItem>>
{
    public async Task<IReadOnlyCollection<SimpleMessageItem>> ExecuteQueryAsync(GetPinnedMessageListQuery query, CancellationToken cancellationToken)
    {
        if (query.ToPeer.PeerType == PeerType.Channel)
        {
            return await store.FindAsync(p => p.OwnerPeerId == query.ToPeer.PeerId,
                p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId), limit: query.Limit, cancellationToken: cancellationToken);
        }

        if (!query.IncludeOtherParticipantMessages)
        {
            return await store.FindAsync(p => p.OwnerPeerId == query.RequestUserId && p.Pinned && p.ToPeerId == query.ToPeer.PeerId,
                p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
                limit: query.Limit, cancellationToken: cancellationToken
            );
        }

        var batchIds = await store.FindAsync(
            p => p.OwnerPeerId == query.RequestUserId && p.Pinned && p.ToPeerId == query.ToPeer.PeerId,
            p => p.BatchId,
            limit: query.Limit, cancellationToken: cancellationToken);

        if (batchIds.Count == 0)
        {
            return [];
        }

        return await store.FindAsync(p => batchIds.Contains(p.BatchId),
            p => new SimpleMessageItem(p.OwnerPeerId, p.MessageId, p.ToPeerType, p.ToPeerId),
            cancellationToken: cancellationToken);
    }
}