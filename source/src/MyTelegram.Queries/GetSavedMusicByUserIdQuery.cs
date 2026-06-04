namespace MyTelegram.Queries;

public class GetSavedMusicByUserIdQuery(long userId) : IQuery<ISavedMusicReadModel?>
{
    public long UserId { get; } = userId;
}
