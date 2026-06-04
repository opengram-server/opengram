using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.ReadModel.Interfaces;

public interface IUserReadModel : IReadModel
{
    string? About { get; }
    long AccessHash { get; }
    int AccountTtl { get; }
    bool Bot { get; }
    int? BotInfoVersion { get; }
    string FirstName { get; }
    bool HasPassword { get; }
    string Id { get; }
    bool IsOnline { get; }
    string? LastName { get; }
    DateTime LastUpdateDate { get; }
    string PhoneNumber { get; }
    string? PhoneCountryCode { get; }
    int? PinnedMsgId { get; }
    List<int> PinnedMsgIdList { get; } // = new();
    //byte[]? ProfilePhoto { get; }
    bool SensitiveCanChange { get; } //= true;
    bool SensitiveEnabled { get; }
    bool ShowContactSignUpNotification { get; }
    bool Support { get; }
    long UserId { get; }
    string? UserName { get; }
    bool Verified { get; }
    bool Restricted { get; }
    string? RestrictionReason { get; }
    bool Premium { get; }
    BusinessWorkHours? BusinessWorkHours { get; }
    BusinessLocation? BusinessLocation { get; }
    BusinessGreetingMessage? BusinessGreetingMessage { get; }
    BusinessAwayMessage? BusinessAwayMessage { get; }
    BusinessIntro? BusinessIntro { get; }
    string? Email { get; }
    long? EmojiStatusDocumentId { get; }
    int? EmojiStatusValidUntil { get; }
    long? EmojiStatusCollectibleId { get; }
    List<long> RecentEmojiStatuses { get; }
    VideoSizeEmojiMarkup? VideoEmojiMarkup { get; }

    long? ProfilePhotoId { get; }
    long? PersonalPhotoId { get; }
    long? FallbackPhotoId { get; }
    //int? Color { get; }
    //long? BackgroundEmojiId { get; }
    PeerColor? Color { get; }
    PeerColor? ProfileColor { get; }
    GlobalPrivacySettings? GlobalPrivacySettings { get; }
    long? PersonalChannelId { get; }
    Birthday? Birthday { get; }

    bool BotHasMainApp { get; }
    int? BotActiveUsers { get; }

    List<string>? Usernames { get; }
    DateTime? CreationTime { get; }
    int? ProfilePhotoUpdateDate { get; }
    int? UserNameUpdateDate { get; }
    bool? IsDeleted { get; }

    // Third-party verification
    long? BotVerificationIcon { get; }
    long? BotVerifierId { get; }
    
    // Auto-delete messages TTL
    int? DefaultHistoryTTL { get; }
    
    // Frozen account animation
    long? FrozenAnimationDocumentId { get; }
    
    // Frozen Account fields
    bool IsFrozen { get; }
    int? FreezeSinceDate { get; }
    int? FreezeUntilDate { get; }
    Domain.FreezeReason? FreezeReason { get; }
    string? FreezeAppealUrl { get; }
}