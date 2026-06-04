namespace MyTelegram.ReadModel.Interfaces;

public interface ICollectibleUsernameReadModel : IReadModel
{
    PeerType PeerType { get; }
    long OwnerPeerId { get; }
    string UserName { get; }
    bool Active { get; }
    int SortingId { get; }
}