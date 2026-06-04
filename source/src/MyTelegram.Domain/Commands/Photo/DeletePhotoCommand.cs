namespace MyTelegram.Domain.Commands.Photo;

public class DeletePhotoCommand(PhotoId aggregateId, long userId/*, long accessHash*/)
    : Command<PhotoAggregate, PhotoId, IExecutionResult>(aggregateId)
{
    public long UserId { get; } = userId;
    //public long AccessHash { get; } = accessHash;
}