using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Commands.ChatImport;

public class UploadChatImportMediaCommand : Command<ChatImportAggregate, ChatImportId, IExecutionResult>
{
    public long ChatId { get; }
    public long ImportId { get; }
    public string FileName { get; }
    public byte[] MediaBytes { get; }

    public UploadChatImportMediaCommand(ChatImportId aggregateId, long chatId, long importId, string fileName, byte[] mediaBytes) : base(aggregateId)
    {
        ChatId = chatId;
        ImportId = importId;
        FileName = fileName;
        MediaBytes = mediaBytes;
    }
}
