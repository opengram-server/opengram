#pragma warning disable CS8618

using MyTelegram.Domain.Events.Privacy;

namespace MyTelegram.Domain.Aggregates.User;

public class UserState : AggregateState<UserAggregate, UserId, UserState>,
    IApply<UserCreatedEvent>,
    IApply<UserProfileUpdatedEvent>,
    IApply<CheckUserStatusCompletedEvent>,
    IApply<UserSupportHasSetEvent>,
    IApply<UserVerifiedHasSetEvent>,
    IApply<UserNameUpdatedEvent>,
    IApply<UserProfilePhotoChangedEvent>,
    IApply<UserProfilePhotoUploadedEvent>,
    IApply<UserColorUpdatedEvent>,
    IApply<UserGlobalPrivacySettingsChangedEvent>,
    IApply<UserPremiumStatusChangedEvent>,
    IApply<PersonalChannelUpdatedEvent>,
    IApply<BirthdayUpdatedEvent>,
    IApply<UserAboutUpdatedEvent>,
    IApply<UserFirstNameUpdatedEvent>,
    IApply<PrivacyRulesUpdatedEvent>,
    IApply<BotVerifierCreatedEvent>,
    IApply<BotVerifierSettingsUpdatedEvent>,
    IApply<CustomVerificationSetEvent>,
    IApply<CustomVerificationRemovedEvent>,
    IApply<UserEmojiStatusUpdatedEvent>,
    IApply<StickerSetInstalledEvent>,
    IApply<BusinessWorkHoursUpdatedEvent>,
    IApply<BusinessLocationUpdatedEvent>,
    IApply<BusinessGreetingMessageUpdatedEvent>,
    IApply<BusinessIntroUpdatedEvent>,
    IApply<BusinessChatLinkCreatedEvent>,
    IApply<BusinessChatLinkDeletedEvent>,
    IApply<UserPasswordStatusUpdatedEvent>,
    IApply<UserAccountFrozenEvent>,
    IApply<UserAccountUnfrozenEvent>,
    IApply<UserFrozenAppealSubmittedEvent>,
    IApply<UserFrozenAppealReviewedEvent>,
    IApply<UserAccountAutoDeletedEvent>
{
    public long AccessHash { get; private set; }
    public string FirstName { get; private set; } = null!;
    public bool HasPassword { get; private set; }
    public bool IsOnline { get; private set; }
    public string? LastName { get; private set; }
    public string PhoneNumber { get; private set; } = string.Empty;
    public byte[]? Photo { get; private set; }
    public bool SensitiveEnabled { get; private set; }
    public bool Support { get; private set; }
    public long UserId { get; private set; }
    public string? UserName { get; private set; }
    public bool Verified { get; private set; }
    public bool IsBot { get; private set; }
    public bool IsDeleted { get; private set; }
    //public bool IsPremium { get; private set; }
    public long? EmojiStatusDocumentId { get; private set; }
    public int? EmojiStatusValidUntil { get; private set; }
    public long? PhotoId { get; private set; }
    public long? FallbackPhotoId { get; private set; }
    public MyTelegram.Domain.Shared.CircularBuffer<long> RecentEmojiStatus { get; private set; } = new(10);
    //public int Color { get; private set; }
    //public long? BackgroundEmojiId { get; private set; }

    public PeerColor? Color { get; private set; }
    public PeerColor? ProfileColor { get; private set; }
    public GlobalPrivacySettings GlobalPrivacySettings { get; private set; }
    public bool Premium { get; private set; }
    public long? PersonalChannelId { get; private set; }
    public Birthday? Birthday { get; private set; }
    public int? ProfilePhotoUpdateDate { get; private set; }
    public int? UserNameUpdateDate { get; private set; }
    public int? DefaultHistoryTTL { get; private set; }
    public List<long> InstalledStickerSetIds { get; private set; } = new();
    
    // Frozen Account fields
    public bool IsFrozen { get; private set; }
    public int? FreezeSinceDate { get; private set; }
    public int? FreezeUntilDate { get; private set; }
    public FreezeReason? FreezeReason { get; private set; }
    public string? FreezeAppealUrl { get; private set; }

    public void Apply(CheckUserStatusCompletedEvent aggregateEvent)
    {
        //throw new NotImplementedException();
    }

    public void Apply(UserCreatedEvent aggregateEvent)
    {
        UserId = aggregateEvent.UserId;
        PhoneNumber = aggregateEvent.PhoneNumber;
        FirstName = aggregateEvent.FirstName;
        LastName = aggregateEvent.LastName;
        SensitiveEnabled = true;
        AccessHash = aggregateEvent.AccessHash;
        IsBot = aggregateEvent.Bot;
        UserName = aggregateEvent.UserName;
        if (!string.IsNullOrEmpty(aggregateEvent.UserName))
        {
            UserNameUpdateDate = aggregateEvent.CreationTime.ToTimestamp();
        }
    }

    public void Apply(UserNameUpdatedEvent aggregateEvent)
    {
        UserName = aggregateEvent.UserItem.UserName;
        UserNameUpdateDate = aggregateEvent.Date;
    }
   
    public void Apply(UserProfilePhotoChangedEvent aggregateEvent)
    {
        //Photo = aggregateEvent.UserItem.ProfilePhoto;
        if (aggregateEvent.Fallback)
        {
            FallbackPhotoId = aggregateEvent.PhotoId;
        }
        else
        {
            PhotoId = aggregateEvent.PhotoId;
        }
    }

    public void Apply(UserProfileUpdatedEvent aggregateEvent)
    {
        if (!string.IsNullOrEmpty(aggregateEvent.FirstName))
        {
            FirstName = aggregateEvent.FirstName;
        }

        LastName = aggregateEvent.LastName;
    }

    public void Apply(UserSupportHasSetEvent aggregateEvent)
    {
        Support = aggregateEvent.Support;
    }

    public void Apply(UserVerifiedHasSetEvent aggregateEvent)
    {
        Verified = aggregateEvent.Verified;
    }
    public void LoadFromSnapshot(UserSnapshot snapshot)
    {
        UserId = snapshot.UserId;
        IsOnline = snapshot.IsOnline;

        AccessHash = snapshot.AccessHash;
        FirstName = snapshot.FirstName;
        LastName = snapshot.LastName;
        PhoneNumber = snapshot.PhoneNumber;
        HasPassword = snapshot.HasPassword;
        UserName = snapshot.UserName;
        Photo = snapshot.Photo;
        IsBot = snapshot.IsBot;
        IsDeleted = snapshot.IsDeleted;

        EmojiStatusDocumentId = snapshot.EmojiStatusDocumentId;
        EmojiStatusValidUntil = snapshot.EmojiStatusValidUntil;
        RecentEmojiStatus = new MyTelegram.Domain.Shared.CircularBuffer<long>(10, snapshot.RecentEmojiStatuses.ToArray());

        PhotoId = snapshot.PhotoId;
        FallbackPhotoId = snapshot.FallbackPhotoId;
        Color = snapshot.Color;
        ProfileColor = snapshot.ProfileColor;
        GlobalPrivacySettings = snapshot.GlobalPrivacySettings;
        Premium = snapshot.Premium;
        PersonalChannelId = snapshot.PersonalChannelId;
        Birthday = snapshot.Birthday;
        ProfilePhotoUpdateDate = snapshot.ProfilePhotoUpdateDate;
        UserNameUpdateDate = snapshot.UserNameUpdateDate;
    }

    public void Apply(UserProfilePhotoUploadedEvent aggregateEvent)
    {
        PhotoId = aggregateEvent.PhotoId;
    }

    public void Apply(UserColorUpdatedEvent aggregateEvent)
    {
        if (aggregateEvent.ForProfile)
        {
            ProfileColor = aggregateEvent.Color;
        }
        else
        {
            Color = aggregateEvent.Color;
        }
    }
    public void Apply(UserGlobalPrivacySettingsChangedEvent aggregateEvent)
    {
        GlobalPrivacySettings= aggregateEvent.GlobalPrivacySettings;
    }
    public void Apply(UserPremiumStatusChangedEvent aggregateEvent)
    {
        Premium=aggregateEvent.Premium;
    }
    public void Apply(PersonalChannelUpdatedEvent aggregateEvent)
    {
        PersonalChannelId = aggregateEvent.PersonalChannelId;
    }
    public void Apply(BirthdayUpdatedEvent aggregateEvent)
    {
        Birthday = aggregateEvent.Birthday;
    }

    public void Apply(UserAboutUpdatedEvent aggregateEvent)
    {
        
    }

    public void Apply(UserFirstNameUpdatedEvent aggregateEvent)
    {
        FirstName= aggregateEvent.FirstName;
    }

    public void Apply(PrivacyRulesUpdatedEvent aggregateEvent)
    {
        // Privacy rules are stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(BotVerifierCreatedEvent aggregateEvent)
    {
        // Bot verifier data is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(BotVerifierSettingsUpdatedEvent aggregateEvent)
    {
        // Bot verifier data is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(CustomVerificationSetEvent aggregateEvent)
    {
        // Custom verification data is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(CustomVerificationRemovedEvent aggregateEvent)
    {
        // Custom verification data is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(UserEmojiStatusUpdatedEvent aggregateEvent)
    {
        EmojiStatusDocumentId = aggregateEvent.EmojiStatusDocumentId;
        EmojiStatusValidUntil = aggregateEvent.EmojiStatusValidUntil;
        
        // Add to recent emoji statuses if it's a valid emoji (not empty)
        if (aggregateEvent.EmojiStatusDocumentId.HasValue && aggregateEvent.EmojiStatusDocumentId.Value > 0)
        {
            RecentEmojiStatus.Put(aggregateEvent.EmojiStatusDocumentId.Value);
        }
    }

    public void Apply(UserDefaultHistoryTTLUpdatedEvent aggregateEvent)
    {
        DefaultHistoryTTL = aggregateEvent.Period;
    }

    public void Apply(StickerSetInstalledEvent aggregateEvent)
    {
        if (!InstalledStickerSetIds.Contains(aggregateEvent.StickerSetId))
        {
            InstalledStickerSetIds.Add(aggregateEvent.StickerSetId);
        }
    }

    public void Apply(BusinessWorkHoursUpdatedEvent aggregateEvent)
    {
        // Business work hours updated - handled by read model
    }

    public void Apply(BusinessLocationUpdatedEvent aggregateEvent)
    {
        // Business location updated - handled by read model
    }

    public void Apply(BusinessGreetingMessageUpdatedEvent aggregateEvent)
    {
        // Business greeting message updated - handled by read model
    }

    public void Apply(BusinessIntroUpdatedEvent aggregateEvent)
    {
        // Business intro updated - handled by read model
    }

    public void Apply(BusinessChatLinkCreatedEvent aggregateEvent)
    {
        // Business chat link created - handled by read model
    }

    public void Apply(BusinessChatLinkDeletedEvent aggregateEvent)
    {
        // Business chat link deleted - handled by read model
    }

    public void Apply(UserPasswordStatusUpdatedEvent aggregateEvent)
    {
        HasPassword = aggregateEvent.HasPassword;
    }

    public void Apply(UserAccountFrozenEvent aggregateEvent)
    {
        IsFrozen = true;
        FreezeSinceDate = aggregateEvent.FreezeSinceDate;
        FreezeUntilDate = aggregateEvent.FreezeUntilDate;
        FreezeReason = aggregateEvent.Reason;
        FreezeAppealUrl = aggregateEvent.AppealUrl;
    }

    public void Apply(UserAccountUnfrozenEvent aggregateEvent)
    {
        IsFrozen = false;
        FreezeSinceDate = null;
        FreezeUntilDate = null;
        FreezeReason = null;
        FreezeAppealUrl = null;
    }

    public void Apply(UserFrozenAppealSubmittedEvent aggregateEvent)
    {
        // Appeal data is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(UserFrozenAppealReviewedEvent aggregateEvent)
    {
        // Appeal review is stored in separate read model
        // No state change needed in aggregate
    }

    public void Apply(UserAccountAutoDeletedEvent aggregateEvent)
    {
        IsDeleted = true;
        IsFrozen = false;
    }
}
