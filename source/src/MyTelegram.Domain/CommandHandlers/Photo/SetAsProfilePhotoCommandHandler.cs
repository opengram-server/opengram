namespace MyTelegram.Domain.CommandHandlers.Photo;

public class SetAsProfilePhotoCommandHandler : CommandHandler<PhotoAggregate, PhotoId, SetAsProfilePhotoCommand>
{
    public override Task ExecuteAsync(PhotoAggregate aggregate,
        SetAsProfilePhotoCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.SetAsProfilePhoto();

        return Task.CompletedTask;
    }
}