using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;

namespace MyTelegram.Domain.CommandHandlers.ChatImport;

public class CreateChatImportCommandHandler : CommandHandler<ChatImportAggregate, ChatImportId, CreateChatImportCommand>
{
    public override Task ExecuteAsync(ChatImportAggregate aggregate, CreateChatImportCommand command, CancellationToken cancellationToken)
    {
        aggregate.Create(command.ChatId, command.Title, command.UserId);
        return Task.CompletedTask;
    }
}
