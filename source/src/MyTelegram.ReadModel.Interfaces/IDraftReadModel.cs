namespace MyTelegram.ReadModel.Interfaces;

public interface IDraftReadModel : IReadModel
{
    Draft Draft { get; }
    string Id { get; }
    long OwnerPeerId { get; }
    Peer Peer { get; }
}