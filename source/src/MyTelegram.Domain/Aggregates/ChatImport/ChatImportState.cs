using MyTelegram.Domain.Events.ChatImport;
namespace MyTelegram.Domain.Aggregates.ChatImport;

public class ChatImportState : AggregateState<ChatImportAggregate, ChatImportId, ChatImportState>,
    IApply<ChatImportCreatedEvent>,
    IApply<ChatImportParsedEvent>
{
    public long ChatId { get; private set; }
    public string Title { get; private set; } = null!;
    public int PeerType { get; private set; }

    public void Apply(ChatImportCreatedEvent aggregateEvent)
    {
        ChatId = aggregateEvent.ChatId;
        Title = aggregateEvent.Title;
    }

    public void Apply(ChatImportParsedEvent aggregateEvent)
    {
        // Update state if needed
    }
}
