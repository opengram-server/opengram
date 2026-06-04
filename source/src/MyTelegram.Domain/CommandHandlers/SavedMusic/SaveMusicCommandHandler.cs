using MyTelegram.Domain.Aggregates.SavedMusic;
using MyTelegram.Domain.Commands.SavedMusic;

namespace MyTelegram.Domain.CommandHandlers.SavedMusic;

public class SaveMusicCommandHandler : CommandHandler<SavedMusicAggregate, SavedMusicId, SaveMusicCommand>
{
    public override Task ExecuteAsync(
        SavedMusicAggregate aggregate,
        SaveMusicCommand command,
        CancellationToken cancellationToken)
    {
        if (aggregate.IsNew)
        {
            aggregate.Create(command.UserId);
        }

        if (command.Unsave)
        {
            aggregate.RemoveMusic(command.DocumentId, command.RequestInfo);
        }
        else
        {
            aggregate.AddMusic(command.DocumentId, command.AfterDocumentId, command.RequestInfo);
        }

        return Task.CompletedTask;
    }
}
