using MyTelegram.Domain.Aggregates.Chat;

namespace MyTelegram.Domain.Events.Chat;

public class ChatCreatedEvent(
    long chatId,
    string title,
    long creatorId,
    List<long> memberUidList,
    int date) : AggregateEvent<ChatAggregate, ChatId>
{
    public long ChatId { get; } = chatId;
    public string Title { get; } = title;
    public long CreatorId { get; } = creatorId;
    public List<long> MemberUidList { get; } = memberUidList;
    public int Date { get; } = date;
}
