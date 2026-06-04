using MyTelegram.Domain.Events.ChatImport;

namespace MyTelegram.Domain.Aggregates.ChatImport;

public class ChatImportAggregate : AggregateRoot<ChatImportAggregate, ChatImportId>
{
    private readonly ChatImportState _state = new();

    public ChatImportAggregate(ChatImportId id) : base(id)
    {
        Register(_state);
    }

    public void Create(long chatId, string title, long userId)
    {
        Emit(new ChatImportCreatedEvent(chatId, title, userId));
    }

    public void UploadMedia(long importId, string fileName)
    {
        Emit(new ChatImportMediaUploadedEvent(importId, fileName));
    }

    public void StartImport(long importId)
    {
        Emit(new ChatImportStartedEvent(importId));
    }

    public void Parse()
    {
        // Logic for parsing
        Emit(new ChatImportParsedEvent(0, "Title", DateTime.UtcNow));
    }


}
