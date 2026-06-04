using MyTelegram.Domain.Aggregates.ChatImport;
namespace MyTelegram.Domain.Commands.ChatImport;

public class CreateChatImportCommand : Command<ChatImportAggregate, ChatImportId, IExecutionResult>
{
    public long ChatId { get; }
    public string Title { get; }
    public long UserId { get; }

    public CreateChatImportCommand(ChatImportId aggregateId, long chatId, string title, long userId) : base(aggregateId)
    {
        ChatId = chatId;
        Title = title;
        UserId = userId;
    }
}
