using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Events.ChatImport;

public class ChatImportStartedEvent : AggregateEvent<ChatImportAggregate, ChatImportId>
{
    public long ImportId { get; }

    public ChatImportStartedEvent(long importId)
    {
        ImportId = importId;
    }
}
