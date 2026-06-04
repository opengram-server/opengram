namespace MyTelegram.ReadModel;

public interface ISavedMusicReadModel : IReadModel
{
    long UserId { get; }
    List<long> DocumentIds { get; }
}
