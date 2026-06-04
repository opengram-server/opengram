namespace MyTelegram.ReadModel.Interfaces;

public interface IStoryMaxIdReadModel : IReadModel
{
    long UserId { get; }
    long OwnerPeerId { get; }
    int MaxId { get; }
}