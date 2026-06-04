using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Commands.ChatImport;

public class StartChatImportCommand : Command<ChatImportAggregate, ChatImportId, IExecutionResult>
{
    public long ChatId { get; }
    public long ImportId { get; }

    public StartChatImportCommand(ChatImportId aggregateId, long chatId, long importId) : base(aggregateId)
    {
        ChatId = chatId;
        ImportId = importId;
    }
}
