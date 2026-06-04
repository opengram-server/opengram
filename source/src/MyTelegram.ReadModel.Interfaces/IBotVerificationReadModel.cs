namespace MyTelegram.ReadModel.Interfaces;

public interface IBotVerificationReadModel : IReadModel
{
    string Id { get; }
    long? Version { get; }
    long BotVerifierId { get; }
    VerificationTargetType TargetType { get; }
    long TargetId { get; }
    long IconEmojiId { get; }
    string Description { get; }
    string? CustomDescription { get; }
    DateTime VerifiedAt { get; }
    DateTime? UpdatedAt { get; }
}

public enum VerificationTargetType
{
    User = 1,
    Channel = 2
}
