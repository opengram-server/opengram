namespace MyTelegram.Domain.CommandHandlers.Contact;

public class
    UpdateContactProfilePhotoCommandHandler : CommandHandler<ContactAggregate, ContactId,
        UpdateContactProfilePhotoCommand>
{
    public override Task ExecuteAsync(ContactAggregate aggregate, UpdateContactProfilePhotoCommand command,
        CancellationToken cancellationToken)
    {
        aggregate.UpdateProfilePhoto(command.RequestInfo, command.SelfUserId, command.TargetUserId, command.PhotoId,
            command.Suggest, command.SuggestPhoto);
        return Task.CompletedTask;
    }
}