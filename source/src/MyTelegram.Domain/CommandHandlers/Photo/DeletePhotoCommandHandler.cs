namespace MyTelegram.Domain.CommandHandlers.Photo;

public class DeletePhotoCommandHandler : CommandHandler<PhotoAggregate, PhotoId, DeletePhotoCommand>
{
    public override Task ExecuteAsync(PhotoAggregate aggregate,
        DeletePhotoCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Delete(command.UserId);

        return Task.CompletedTask;
    }
}