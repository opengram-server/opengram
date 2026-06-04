namespace MyTelegram.QueryHandlers.MongoDB.Messaging;

// ReSharper disable once UnusedMember.Global
public class GetMessageViewsQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store) : IQueryHandler<GetMessageViewsQuery, IReadOnlyCollection<MessageView>>
{
    public async Task<IReadOnlyCollection<MessageView>> ExecuteQueryAsync(GetMessageViewsQuery query,
        CancellationToken cancellationToken)
    {
        return await store
            .FindAsync(p => p.OwnerPeerId == query.ChannelId && query.MessageIdList.Contains(p.MessageId),
                p => new MessageView
                {
                    MessageId = p.MessageId,
                    Views = p.Views ?? 0
                }, cancellationToken: cancellationToken);
    }
}
