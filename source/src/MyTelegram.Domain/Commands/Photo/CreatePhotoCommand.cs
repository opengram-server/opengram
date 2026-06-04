namespace MyTelegram.Domain.Commands.Photo;

public class CreatePhotoCommand : Command<PhotoAggregate, PhotoId, IExecutionResult>
{
    public CreatePhotoCommand(PhotoId aggregateId, long userId, PhotoItem photo) : base(aggregateId)
    {
        UserId = userId;
        Photo = photo;

        if (photo.Sizes?.Count == 0)
        {
        }
    }

    public PhotoItem Photo { get; }
    public long UserId { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return BitConverter.GetBytes(Photo.Id);
    }
}