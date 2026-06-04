namespace MyTelegram.ReadModel.Interfaces;

public interface IEncryptedMessageReadModel : IReadModel
{
    long ChatId { get; }
    long UserId { get; }
    long PermAuthKeyId { get; }
    byte[] Data { get; }
    byte[]? File { get; }
    int Date { get; }
    string Id { get; }
    SendMessageType MessageType { get; }
    int Qts { get; }
    long RandomId { get; }
}