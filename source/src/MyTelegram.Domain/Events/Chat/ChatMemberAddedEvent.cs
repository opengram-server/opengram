using MyTelegram.Domain.Aggregates.Chat;

namespace MyTelegram.Domain.Events.Chat;

public class ChatMemberAddedEvent(
    long userId,
    long inviterId,
    int date) : AggregateEvent<ChatAggregate, ChatId>
{
    public long UserId { get; } = userId;
    public long InviterId { get; } = inviterId;
    public int Date { get; } = date;
}
