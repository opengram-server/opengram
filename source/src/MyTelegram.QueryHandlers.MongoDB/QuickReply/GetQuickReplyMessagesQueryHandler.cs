using MyTelegram.Queries;
using MyTelegram.ReadModel.Interfaces;
using MyTelegram.ReadModel.Impl;

namespace MyTelegram.QueryHandlers.MongoDB.QuickReply;

public class GetQuickReplyMessagesQueryHandler(IQueryOnlyReadModelStore<MessageReadModel> store)
    : IQueryHandler<GetQuickReplyMessagesQuery, IReadOnlyCollection<IMessageReadModel>>
{
    public async Task<IReadOnlyCollection<IMessageReadModel>> ExecuteQueryAsync(GetQuickReplyMessagesQuery query,
        CancellationToken cancellationToken)
    {
        Expression<Func<MessageReadModel, bool>> filter = p => p.OwnerPeerId == query.UserId && p.ShortcutId == query.ShortcutId;
        
        if (query.MessageIds != null && query.MessageIds.Count > 0)
        {
            filter = filter.And(p => query.MessageIds.Contains(p.MessageId));
        }

        return await store.FindAsync(filter, cancellationToken: cancellationToken);
    }
}
