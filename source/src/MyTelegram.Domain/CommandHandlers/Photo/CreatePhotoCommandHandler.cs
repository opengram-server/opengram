namespace MyTelegram.Domain.CommandHandlers.Photo;

public class CreatePhotoCommandHandler : CommandHandler<PhotoAggregate, PhotoId, CreatePhotoCommand>
{
    public override Task ExecuteAsync(PhotoAggregate aggregate,
        CreatePhotoCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.Create(command.UserId, command.Photo);

        return Task.CompletedTask;
    }
}