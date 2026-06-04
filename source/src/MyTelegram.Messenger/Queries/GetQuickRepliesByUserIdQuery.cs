using MyTelegram.ReadModel.Interfaces;

namespace MyTelegram.Messenger.Queries;

public class GetQuickRepliesByUserIdQuery(long userId) : IQuery<IQuickReplyReadModel>
{
    public long UserId { get; } = userId;
}
