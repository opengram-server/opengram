namespace MyTelegram.ReadModel.Interfaces;

public interface IStoryViewReadModel : IReadModel
{
    //long UserId { get; }
    long OwnerPeerId { get; }
    int StoryId { get; }
    //int Date { get; }
    int ViewsCount { get; }
    int? ForwardsCount { get; }
    List<IReactionCount>? Reactions { get; }
    int? ReactionsCount { get; }
    List<long>? RecentViewers { get; }
    int ReactionsUniqueCount { get; }
}