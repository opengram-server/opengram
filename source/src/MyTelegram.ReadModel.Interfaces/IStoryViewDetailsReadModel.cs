namespace MyTelegram.ReadModel.Interfaces;

public interface IStoryViewDetailsReadModel : IReadModel
{
    long UserId { get; }
    long OwnerPeerId { get; }
    int StoryId { get; }
    int Date { get; }
    long ReactionId { get; }
    IReaction? Reaction { get; }
    int ReactionDate { get; }
}