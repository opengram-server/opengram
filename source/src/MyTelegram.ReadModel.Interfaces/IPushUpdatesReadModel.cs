namespace MyTelegram.ReadModel.Interfaces;

public interface IPushUpdatesReadModel : IReadModel
{
    long OwnerPeerId { get; }
    long? ExcludeAuthKeyId { get; set; }
    long? OnlySendToThisAuthKeyId { get; }
    int? MessageId { get; }
    int Pts { get; }
    int Date { get; }
    long SeqNo { get; }
    byte[] Updates { get; }
    List<long>? Users { get; }
    List<long>? Chats { get; }
}