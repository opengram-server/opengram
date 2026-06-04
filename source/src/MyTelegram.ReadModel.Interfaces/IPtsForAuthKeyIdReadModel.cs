namespace MyTelegram.ReadModel.Interfaces;

public interface IPtsForAuthKeyIdReadModel : IReadModel
{
    long GlobalSeqNo { get; }
    string Id { get; }
    long PeerId { get; }
    long PermAuthKeyId { get; }
    //int UnreadCount { get; }
    int Pts { get; }
    int Qts { get; }
}