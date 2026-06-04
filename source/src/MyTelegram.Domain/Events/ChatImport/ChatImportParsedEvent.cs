using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Events.ChatImport;

public class ChatImportParsedEvent : AggregateEvent<ChatImportAggregate, ChatImportId>
{
    public long ImportId { get; }
    public string Title { get; }
    public DateTime Date { get; }

    public ChatImportParsedEvent(long importId, string title, DateTime date)
    {
        ImportId = importId;
        Title = title;
        Date = date;
    }
}
