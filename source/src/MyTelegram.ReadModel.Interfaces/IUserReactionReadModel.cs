namespace MyTelegram.ReadModel.Interfaces;

public interface IUserReactionReadModel : IReadModel
{
    long UserId { get; }
    long PeerId { get; }
    int MessageId { get; }
    long ReactionId { get; }
    Reaction Reaction { get; }
}