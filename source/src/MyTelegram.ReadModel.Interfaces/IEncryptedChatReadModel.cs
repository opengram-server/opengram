namespace MyTelegram.ReadModel.Interfaces;

public interface IEncryptedChatReadModel : IReadModel
{
    long AccessHash { get; }
    long AdminPermAuthKeyId { get; }
    long AdminId { get; }

    long ChatId { get; }
    byte[] Ga { get; }
    byte[] Gb { get; }
    string Id { get; }
    long KeyFingerprint { get; }
    long ParticipantPermAuthKeyId { get; }
    long ParticipantId { get; }
    long RandomId { get; }
}