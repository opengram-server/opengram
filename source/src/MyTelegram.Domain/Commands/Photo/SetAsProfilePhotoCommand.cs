namespace MyTelegram.Domain.Commands.Photo;

public class SetAsProfilePhotoCommand(PhotoId aggregateId)
    : Command<PhotoAggregate, PhotoId, IExecutionResult>(aggregateId)
{
}