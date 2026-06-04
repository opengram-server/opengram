namespace MyTelegram.ReadModel.Interfaces;

public interface IStoryReadingHistoryReadModel : IReadModel
{
    long UserId { get; }
    long StoryOwnerPeerId { get; }
    int StoryId { get; }
    int Date { get; }
}