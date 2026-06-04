using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Commands.ChatImport;

public class ImportMessageCommand : Command<ChatImportAggregate, ChatImportId, IExecutionResult>
{
    public long ChatId { get; }
    public long UserId { get; }
    public string Message { get; }
    public int Date { get; }

    public ImportMessageCommand(ChatImportId aggregateId, long chatId, long userId, string message, int date) : base(aggregateId)
    {
        ChatId = chatId;
        UserId = userId;
        Message = message;
        Date = date;
    }
}
