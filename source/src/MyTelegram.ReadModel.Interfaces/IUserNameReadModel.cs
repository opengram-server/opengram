namespace MyTelegram.ReadModel.Interfaces;

public interface IUserNameReadModel : IReadModel
{
    string Id { get; }
    long PeerId { get; }
    PeerType PeerType { get; }
    string UserName { get; }
    int Date { get; }
}