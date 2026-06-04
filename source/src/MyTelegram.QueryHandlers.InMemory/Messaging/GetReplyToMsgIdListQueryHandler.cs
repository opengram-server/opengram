namespace MyTelegram.QueryHandlers.InMemory.Messaging;

public class
    GetReplyToMsgIdListQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetReplyToMsgIdListQuery, IReadOnlyCollection<ReplyToMsgItem>?>
{
    public async Task<IReadOnlyCollection<ReplyToMsgItem>?> ExecuteQueryAsync(GetReplyToMsgIdListQuery query, CancellationToken cancellationToken)
    {
        if (!query.ReplyToMsgId.HasValue)
        {
            return null;
        }

        switch (query.ToPeer.PeerType)
        {
            case PeerType.User:
                {
                    var messageReadModel = await store.FirstOrDefaultAsync(p =>
                        p.OwnerPeerId == query.SelfUserId && p.ToPeerId == query.ToPeer.PeerId &&
                        p.MessageId == query.ReplyToMsgId, cancellationToken: cancellationToken);

                    if (messageReadModel == null)
                    {
                        return null;
                    }

                    // Reply to a message sent by ToPeerId
                    if (!messageReadModel.Out)
                    {
                        return new[] { new ReplyToMsgItem(messageReadModel.SenderPeerId, messageReadModel.SenderMessageId) };
                    }

                    // Reply to a message sent by myself
                    var item = await store.FirstOrDefaultAsync(p =>
                        p.OwnerPeerId == query.ToPeer.PeerId && p.ToPeerId == query.SelfUserId &&
                        p.SenderMessageId == query.ReplyToMsgId,
                        p => new ReplyToMsgItem(p.OwnerPeerId, p.MessageId), cancellationToken: cancellationToken);

                    if (item != null)
                    {
                        return [item];
                    }

                    return [];
                }
            case PeerType.Chat:
                {
                    var selfMessageReadModel = await store.FirstOrDefaultAsync(p =>
                        p.ToPeerId == query.ToPeer.PeerId && p.OwnerPeerId ==
                        query.SelfUserId && p.MessageId == query.ReplyToMsgId, cancellationToken: cancellationToken);

                    if (selfMessageReadModel == null)
                    {
                        return null;
                    }

                    var senderUserId = selfMessageReadModel.SenderPeerId;
                    var senderMessageId = selfMessageReadModel.SenderMessageId;

                    return await store.FindAsync(p =>
                        p.ToPeerId == query.ToPeer.PeerId && p.SenderPeerId == senderUserId &&
                        p.SenderMessageId == senderMessageId, p => new ReplyToMsgItem(p.OwnerPeerId, p.MessageId), cancellationToken: cancellationToken);
                }
            case PeerType.Channel:
                {
                    return await store.FindAsync(p =>
                        p.OwnerPeerId == query.ToPeer.PeerId &&
                        p.MessageId == query.ReplyToMsgId, p => new ReplyToMsgItem(p.OwnerPeerId, p.MessageId), cancellationToken: cancellationToken);
                }
        }
        return null;
    }
}