using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;
using MyTelegram.Domain.Events.ChatImport;

namespace MyTelegram.Domain.CommandHandlers.ChatImport;

public class StartChatImportCommandHandler : CommandHandler<ChatImportAggregate, ChatImportId, StartChatImportCommand>
{
    public override Task ExecuteAsync(ChatImportAggregate aggregate, StartChatImportCommand command, CancellationToken cancellationToken)
    {
        aggregate.StartImport(command.ImportId);
        return Task.CompletedTask;
    }
}
