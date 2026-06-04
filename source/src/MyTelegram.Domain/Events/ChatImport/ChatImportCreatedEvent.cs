using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Events.ChatImport;

public class ChatImportCreatedEvent : AggregateEvent<ChatImportAggregate, ChatImportId>
{
    public long ChatId { get; }
    public string Title { get; }
    public long UserId { get; }

    public ChatImportCreatedEvent(long chatId, string title, long userId)
    {
        ChatId = chatId;
        Title = title;
        UserId = userId;
    }
}
