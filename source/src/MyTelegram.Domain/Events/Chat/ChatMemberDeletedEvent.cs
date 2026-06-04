using MyTelegram.Domain.Aggregates.Chat;

namespace MyTelegram.Domain.Events.Chat;

public class ChatMemberDeletedEvent(long userId) : AggregateEvent<ChatAggregate, ChatId>
{
    public long UserId { get; } = userId;
}
