namespace MyTelegram.ReadModel.Interfaces;

public interface IEncryptedPushUpdatesReadModel : IReadModel
{
    byte[] Data { get; }
    string Id { get; }
    long InboxOwnerPermAuthKeyId { get; }
    long InboxOwnerPeerId { get; }
    int Qts { get; }
}