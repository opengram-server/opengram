using MyTelegram.Domain.Aggregates.ChatImport;
using MyTelegram.Domain.Commands.ChatImport;
using MyTelegram.Domain.Events.ChatImport;

namespace MyTelegram.Domain.CommandHandlers.ChatImport;

public class UploadChatImportMediaCommandHandler : CommandHandler<ChatImportAggregate, ChatImportId, UploadChatImportMediaCommand>
{
    public override Task ExecuteAsync(ChatImportAggregate aggregate, UploadChatImportMediaCommand command, CancellationToken cancellationToken)
    {
        aggregate.UploadMedia(command.ImportId, command.FileName);
        return Task.CompletedTask;
    }
}
