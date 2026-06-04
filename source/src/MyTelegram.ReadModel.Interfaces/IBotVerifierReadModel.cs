namespace MyTelegram.ReadModel.Interfaces;

public interface IBotVerifierReadModel : IReadModel
{
    string Id { get; }
    long? Version { get; }
    long BotUserId { get; }
    long IconEmojiId { get; }
    string CompanyName { get; }
    bool CanModifyCustomDescription { get; }
    bool IsActive { get; }
    DateTime CreatedAt { get; }
    DateTime? UpdatedAt { get; }
}
