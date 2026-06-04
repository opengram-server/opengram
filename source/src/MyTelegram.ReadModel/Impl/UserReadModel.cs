using MyTelegram.Domain.Extensions;
using MyTelegram.Domain.Shared.Business;

namespace MyTelegram.ReadModel.Impl;

public class UserReadModel : IUserReadModel,
    IAmReadModelFor<UserAggregate, UserId, UserCreatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserProfileUpdatedEvent>,
    IAmReadModelFor<MessageAggregate, MessageId, OutboxMessagePinnedUpdatedEvent>,
    IAmReadModelFor<MessageAggregate, MessageId, InboxMessagePinnedUpdatedEvent>,
    IAmReadModelFor<MessageAggregate, MessageId, MessagePinnedUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserSupportHasSetEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserVerifiedHasSetEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserNameUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserProfilePhotoChangedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserProfilePhotoUploadedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserColorUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserGlobalPrivacySettingsChangedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserPremiumStatusChangedEvent>,
    IAmReadModelFor<UserAggregate, UserId, PersonalChannelUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BirthdayUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserAboutUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserFirstNameUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, CustomVerificationSetEvent>,
    IAmReadModelFor<UserAggregate, UserId, CustomVerificationRemovedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserEmojiStatusUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserDefaultHistoryTTLUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessWorkHoursUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessLocationUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessGreetingMessageUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessAwayMessageUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessIntroUpdatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessChatLinkCreatedEvent>,
    IAmReadModelFor<UserAggregate, UserId, BusinessChatLinkDeletedEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserAccountFrozenEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserAccountUnfrozenEvent>,
    IAmReadModelFor<UserAggregate, UserId, UserAccountAutoDeletedEvent>
{
    public virtual string? About { get; private set; }
    public virtual long AccessHash { get; private set; }
    public virtual int AccountTtl { get; private set; }
    public Birthday? Birthday { get; private set; }
    public virtual bool Bot { get; private set; }
    public int? BotActiveUsers { get; private set; }
    public bool BotHasMainApp { get; private set; }
    public int? BotInfoVersion { get; private set; }
    public PeerColor? Color { get; private set; }
    public DateTime? CreationTime { get; private set; }
    public string? Email { get; private set; }
    public long? EmojiStatusDocumentId { get; private set; }
    public int? EmojiStatusValidUntil { get; private set; }
    public long? EmojiStatusCollectibleId { get; private set; }
    public long? FallbackPhotoId { get; private set; }
    public virtual string FirstName { get; private set; } = null!;
    public GlobalPrivacySettings? GlobalPrivacySettings { get; private set; }
    public virtual bool HasPassword { get; private set; }
    public virtual string Id { get; set; } = null!;
    public virtual bool IsOnline { get; private set; }
    public virtual string? LastName { get; private set; }
    public virtual DateTime LastUpdateDate { get; private set; }
    public long? PersonalChannelId { get; private set; }
    public long? PersonalPhotoId { get; private set; }
    public virtual string PhoneNumber { get; private set; } = null!;
    public string? PhoneCountryCode { get; private set; } // e.g. "US", "RU", "GB"
    public virtual int? PinnedMsgId { get; private set; }
    public virtual List<int> PinnedMsgIdList { get; protected set; } = [];
    public bool Premium { get; private set; }
    public PeerColor? ProfileColor { get; private set; }
    public virtual byte[]? ProfilePhoto { get; private set; }
    public long? ProfilePhotoId { get; private set; }
    public int? ProfilePhotoUpdateDate { get; private set; }
    public List<long> RecentEmojiStatuses { get; private set; } = [];
    public virtual bool SensitiveCanChange { get; private set; }
    public virtual bool SensitiveEnabled { get; private set; }
    public virtual bool ShowContactSignUpNotification { get; private set; }
    public virtual bool Support { get; private set; }
    public virtual long UserId { get; set; }
    //public string UserId { get; private set; }
    public virtual string? UserName { get; private set; }

    public List<string>? Usernames { get; private set; }
    public int? UserNameUpdateDate { get; private set; }
    public bool? IsDeleted { get; set; }
    public virtual bool Verified { get; private set; }
    public bool Restricted { get; private set; } // Account is restricted/frozen
    public string? RestrictionReason { get; private set; } // Why account is restricted
    public long? FrozenAnimationDocumentId { get; private set; } // JSON animation for frozen accounts
    
    // Business properties
    public BusinessWorkHours? BusinessWorkHours { get; private set; }
    public BusinessLocation? BusinessLocation { get; private set; }
    public BusinessGreetingMessage? BusinessGreetingMessage { get; private set; }
    public BusinessAwayMessage? BusinessAwayMessage { get; private set; }
    public BusinessIntro? BusinessIntro { get; private set; }
    public List<BusinessChatLink> BusinessChatLinks { get; private set; } = new();

    //public int? Color { get; private set; }
    //public long? BackgroundEmojiId { get; private set; }
    public virtual long? Version { get; set; }

    public VideoSizeEmojiMarkup? VideoEmojiMarkup { get; private set; }
    
    // Third-party verification
    public long? BotVerificationIcon { get; set; }
    public long? BotVerifierId { get; set; }
    
    // Auto-delete messages TTL
    public int? DefaultHistoryTTL { get; private set; }
    
    // Frozen Account fields
    public bool IsFrozen { get; private set; }
    public int? FreezeSinceDate { get; private set; }
    public int? FreezeUntilDate { get; private set; }
    public Domain.FreezeReason? FreezeReason { get; private set; }
    public string? FreezeAppealUrl { get; private set; }

    public Task ApplyAsync(IReadModelContext context,
            IDomainEvent<MessageAggregate, MessageId, InboxMessagePinnedUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.ToPeer.PeerType == PeerType.User)
        {
            UpdatePinnedMsgId(domainEvent.AggregateEvent.MessageId, domainEvent.AggregateEvent.Pinned);
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<MessageAggregate, MessageId, OutboxMessagePinnedUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.ToPeer.PeerType == PeerType.User)
        {
            UpdatePinnedMsgId(domainEvent.AggregateEvent.MessageId, domainEvent.AggregateEvent.Pinned);
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserCreatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Id = domainEvent.AggregateIdentity.Value;
        UserId = domainEvent.AggregateEvent.UserId;
        PhoneNumber = domainEvent.AggregateEvent.PhoneNumber;
        PhoneCountryCode = ExtractCountryCode(domainEvent.AggregateEvent.PhoneNumber);
        FirstName = domainEvent.AggregateEvent.FirstName;
        LastName = domainEvent.AggregateEvent.LastName;
        AccessHash = domainEvent.AggregateEvent.AccessHash;
        LastUpdateDate = domainEvent.AggregateEvent.CreationTime;
        Bot = domainEvent.AggregateEvent.Bot;
        BotInfoVersion = domainEvent.AggregateEvent.BotInfoVersion;
        AccountTtl = domainEvent.AggregateEvent.AccountTtl;
        SensitiveCanChange = true;
        ShowContactSignUpNotification = false;
        UserName = domainEvent.AggregateEvent.UserName;
        CreationTime = domainEvent.AggregateEvent.CreationTime;
        DefaultHistoryTTL = 0; // Default: messages don't auto-delete (0 = disabled)
        if (!string.IsNullOrEmpty(domainEvent.AggregateEvent.UserName))
        {
            UserNameUpdateDate = domainEvent.AggregateEvent.CreationTime.ToTimestamp();
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserNameUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        UserName = domainEvent.AggregateEvent.UserItem.UserName;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserProfilePhotoChangedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        ProfilePhotoId = domainEvent.AggregateEvent.PhotoId;
        if (domainEvent.AggregateEvent.Fallback)
        {
            FallbackPhotoId = domainEvent.AggregateEvent.PhotoId;
        }
        else
        {
            ProfilePhotoId = domainEvent.AggregateEvent.PhotoId;
        }

        ProfilePhotoUpdateDate = domainEvent.AggregateEvent.Date;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserProfileUpdatedEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        FirstName = domainEvent.AggregateEvent.FirstName;
        LastName = domainEvent.AggregateEvent.LastName;

        About = domainEvent.AggregateEvent.About;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserSupportHasSetEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Support = domainEvent.AggregateEvent.Support;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context,
        IDomainEvent<UserAggregate, UserId, UserVerifiedHasSetEvent> domainEvent,
        CancellationToken cancellationToken)
    {
        Verified = domainEvent.AggregateEvent.Verified;

        return Task.CompletedTask;
    }
    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserProfilePhotoUploadedEvent> domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.Fallback)
        {
            FallbackPhotoId = domainEvent.AggregateEvent.PhotoId;
        }
        else
        {
            ProfilePhotoId = domainEvent.AggregateEvent.PhotoId;
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserColorUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        if (domainEvent.AggregateEvent.ForProfile)
        {
            ProfileColor = domainEvent.AggregateEvent.Color;
        }
        else
        {
            Color = domainEvent.AggregateEvent.Color;
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserGlobalPrivacySettingsChangedEvent> domainEvent, CancellationToken cancellationToken)
    {
        GlobalPrivacySettings = domainEvent.AggregateEvent.GlobalPrivacySettings;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserPremiumStatusChangedEvent> domainEvent, CancellationToken cancellationToken)
    {
        Premium = domainEvent.AggregateEvent.Premium;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, PersonalChannelUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        PersonalChannelId = domainEvent.AggregateEvent.PersonalChannelId;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BirthdayUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        Birthday = domainEvent.AggregateEvent.Birthday;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<MessageAggregate, MessageId, MessagePinnedUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        UpdatePinnedMsgId(domainEvent.AggregateEvent.MessageId, domainEvent.AggregateEvent.Pinned);

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAboutUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        About = domainEvent.AggregateEvent.About;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserFirstNameUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        FirstName = domainEvent.AggregateEvent.FirstName;

        return Task.CompletedTask;
    }

    private void UpdatePinnedMsgId(int messageId,
                                            bool pinned)
    {
        if (pinned)
        {
            PinnedMsgId = messageId;
            PinnedMsgIdList.Add(messageId);
        }
        else
        {
            PinnedMsgIdList.Remove(messageId);
            PinnedMsgId = PinnedMsgIdList.LastOrDefault();
        }
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, CustomVerificationSetEvent> domainEvent, CancellationToken cancellationToken)
    {
        var evt = domainEvent.AggregateEvent;
        BotVerificationIcon = evt.IconEmojiId;
        BotVerifierId = evt.BotVerifierId;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, CustomVerificationRemovedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BotVerificationIcon = null;
        BotVerifierId = null;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserEmojiStatusUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        EmojiStatusDocumentId = domainEvent.AggregateEvent.EmojiStatusDocumentId;
        EmojiStatusValidUntil = domainEvent.AggregateEvent.EmojiStatusValidUntil;
        EmojiStatusCollectibleId = domainEvent.AggregateEvent.EmojiStatusCollectibleId;
        
        // Add to recent emoji statuses if it's a valid emoji (not empty)
        if (domainEvent.AggregateEvent.EmojiStatusDocumentId.HasValue && domainEvent.AggregateEvent.EmojiStatusDocumentId.Value > 0)
        {
            if (!RecentEmojiStatuses.Contains(domainEvent.AggregateEvent.EmojiStatusDocumentId.Value))
            {
                RecentEmojiStatuses.Insert(0, domainEvent.AggregateEvent.EmojiStatusDocumentId.Value);
                if (RecentEmojiStatuses.Count > 10)
                {
                    RecentEmojiStatuses.RemoveAt(RecentEmojiStatuses.Count - 1);
                }
            }
        }

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserDefaultHistoryTTLUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        DefaultHistoryTTL = domainEvent.AggregateEvent.Period;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessWorkHoursUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BusinessWorkHours = domainEvent.AggregateEvent.WorkHours;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessLocationUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BusinessLocation = domainEvent.AggregateEvent.Location;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessGreetingMessageUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BusinessGreetingMessage = domainEvent.AggregateEvent.GreetingMessage;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessAwayMessageUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BusinessAwayMessage = domainEvent.AggregateEvent.AwayMessage;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessIntroUpdatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        BusinessIntro = domainEvent.AggregateEvent.BusinessIntro;
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessChatLinkCreatedEvent> domainEvent, CancellationToken cancellationToken)
    {
        if (!BusinessChatLinks.Any(l => l.Id == domainEvent.AggregateEvent.ChatLink.Id))
        {
            BusinessChatLinks.Add(domainEvent.AggregateEvent.ChatLink);
        }
        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, BusinessChatLinkDeletedEvent> domainEvent, CancellationToken cancellationToken)
    {
        var linkToRemove = BusinessChatLinks.FirstOrDefault(l => l.Id == domainEvent.AggregateEvent.LinkId);
        if (linkToRemove != null)
        {
            BusinessChatLinks.Remove(linkToRemove);
        }
        return Task.CompletedTask;
    }

    private static string? ExtractCountryCode(string phoneNumber)
    {
        if (string.IsNullOrEmpty(phoneNumber))
            return null;

        // Simple country code extraction based on common prefixes
        // Format: phone numbers start with country code (1-3 digits)
        var phone = phoneNumber.TrimStart('+');
        
        // Common country codes (simplified mapping)
        if (phone.StartsWith("1")) return "US"; // USA/Canada
        if (phone.StartsWith("7")) return "RU"; // Russia/Kazakhstan
        if (phone.StartsWith("44")) return "GB"; // UK
        if (phone.StartsWith("49")) return "DE"; // Germany
        if (phone.StartsWith("33")) return "FR"; // France
        if (phone.StartsWith("39")) return "IT"; // Italy
        if (phone.StartsWith("34")) return "ES"; // Spain
        if (phone.StartsWith("86")) return "CN"; // China
        if (phone.StartsWith("81")) return "JP"; // Japan
        if (phone.StartsWith("82")) return "KR"; // South Korea
        if (phone.StartsWith("91")) return "IN"; // India
        if (phone.StartsWith("55")) return "BR"; // Brazil
        if (phone.StartsWith("52")) return "MX"; // Mexico
        if (phone.StartsWith("61")) return "AU"; // Australia
        if (phone.StartsWith("380")) return "UA"; // Ukraine
        if (phone.StartsWith("48")) return "PL"; // Poland
        if (phone.StartsWith("90")) return "TR"; // Turkey
        if (phone.StartsWith("98")) return "IR"; // Iran
        if (phone.StartsWith("966")) return "SA"; // Saudi Arabia
        if (phone.StartsWith("971")) return "AE"; // UAE
        
        return "XX"; // Unknown country
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAccountFrozenEvent> domainEvent, CancellationToken cancellationToken)
    {
        IsFrozen = true;
        FreezeSinceDate = domainEvent.AggregateEvent.FreezeSinceDate;
        FreezeUntilDate = domainEvent.AggregateEvent.FreezeUntilDate;
        FreezeReason = domainEvent.AggregateEvent.Reason;
        FreezeAppealUrl = domainEvent.AggregateEvent.AppealUrl;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAccountUnfrozenEvent> domainEvent, CancellationToken cancellationToken)
    {
        IsFrozen = false;
        FreezeSinceDate = null;
        FreezeUntilDate = null;
        FreezeReason = null;
        FreezeAppealUrl = null;

        return Task.CompletedTask;
    }

    public Task ApplyAsync(IReadModelContext context, IDomainEvent<UserAggregate, UserId, UserAccountAutoDeletedEvent> domainEvent, CancellationToken cancellationToken)
    {
        IsDeleted = true;
        IsFrozen = false;

        return Task.CompletedTask;
    }
}
