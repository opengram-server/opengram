using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Events.ChatImport;

public class ChatImportMediaUploadedEvent : AggregateEvent<ChatImportAggregate, ChatImportId>
{
    public long ImportId { get; }
    public string FileName { get; }
    // In a real app, we'd pass the file location/ID, not bytes
    
    public ChatImportMediaUploadedEvent(long importId, string fileName)
    {
        ImportId = importId;
        FileName = fileName;
    }
}
