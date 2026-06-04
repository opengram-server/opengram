using MyTelegram.Domain.Events.Privacy;
using MyTelegram.Domain.Shared.Business;
using MyTelegram.Domain.Events.User;

namespace MyTelegram.Domain.Aggregates.User;

public class UserAggregate : MyInMemorySnapshotAggregateRoot<UserAggregate, UserId, UserSnapshot>
{
    private readonly UserState _state = new();
    private readonly int AccountDefaultTtl = 365;

    public UserAggregate(UserId id) : base(id, SnapshotEveryFewVersionsStrategy.Default)
    {
        Register(_state);
    }


    public void UpdateAbout(string? about)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserAboutUpdatedEvent(_state.UserId, about));
    }

    public void UpdateFirstName(string firstName)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserFirstNameUpdatedEvent(_state.UserId, firstName));
    }

    public void UpdateBirthday(Birthday? birthday)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BirthdayUpdatedEvent(birthday));
    }
    public void UpdatePersonalChannel(RequestInfo requestInfo, long? personalChannelId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new PersonalChannelUpdatedEvent(_state.UserId, personalChannelId));
    }
    public void CheckUserStatus(RequestInfo requestInfo)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        CheckUserDeletionState();

        Emit(new CheckUserStatusCompletedEvent(
            requestInfo,
            _state.UserId,
            _state.AccessHash,
            _state.PhoneNumber,
            _state.FirstName,
            _state.LastName,
            _state.HasPassword,
            false));
    }

    public void UpdatePasswordStatus(bool hasPassword)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserPasswordStatusUpdatedEvent(_state.UserId, hasPassword));
    }

    public void Create(RequestInfo requestInfo,
        long userId,
        long accessHash,
        string phoneNumber,
        string firstName,
        string? lastName = null,
        string? userName = null,
        bool bot = false)
    {
        Specs.AggregateIsNew.ThrowDomainErrorIfNotSatisfied(this);
        Specs.IsNotEmptyOrNull.ThrowDomainErrorIfNotSatisfied(firstName);

        Emit(new UserCreatedEvent(requestInfo,
            userId,
            accessHash,
            phoneNumber,
            firstName,
            lastName,
            userName,
            bot,
            bot ? 0 : null,
            AccountDefaultTtl,
            DateTime.UtcNow
        ));
    }

    public void SetSupport(bool support)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserSupportHasSetEvent(support));
    }

    public void SetVerified(bool verified)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserVerifiedHasSetEvent(verified));
    }

    public void UpdateColor(RequestInfo requestInfo, PeerColor? color, bool forProfile)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserColorUpdatedEvent(requestInfo, _state.UserId, color, forProfile));
    }

    public void UpdateEmojiStatus(RequestInfo requestInfo, long? emojiStatusDocumentId, int? emojiStatusValidUntil, long? emojiStatusCollectibleId = null)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserEmojiStatusUpdatedEvent(requestInfo, _state.UserId, emojiStatusDocumentId, emojiStatusValidUntil, emojiStatusCollectibleId));
    }

    public void UpdateGlobalPrivacySettings(RequestInfo requestInfo, GlobalPrivacySettings globalPrivacySettings)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserGlobalPrivacySettingsChangedEvent(requestInfo, globalPrivacySettings));
    }

    public void SetPrivacyRules(RequestInfo requestInfo, PrivacyType privacyType, IReadOnlyList<PrivacyValueData> rules)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        // Validate rules count (max 1000 according to API docs)
        if (rules.Count > 1000)
        {
            RpcErrors.RpcErrors400.PrivacyTooLong.ThrowRpcError();
        }

        Emit(new PrivacyRulesUpdatedEvent(requestInfo, _state.UserId, privacyType, rules));
    }

    public void UpdateProfile(RequestInfo requestInfo,
        string? firstName,
        string? lastName,
        string? about)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);

        firstName ??= _state.FirstName;
        Emit(new UserProfileUpdatedEvent(requestInfo,
            _state.UserId,
            firstName,
            lastName,
            about));
    }

    public void UpdateProfilePhoto(RequestInfo requestInfo,
            long? photoId,
            bool fallback)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserProfilePhotoChangedEvent(requestInfo,
            _state.UserId,
            photoId,
            fallback,
            _state.IsBot,
            DateTime.UtcNow.ToTimestamp()
            ));
    }

    public void UpdateUserName(RequestInfo requestInfo,
        string userName)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserNameUpdatedEvent(requestInfo,
            new UserItem(_state.UserId,
                _state.AccessHash,
                _state.PhoneNumber,
                _state.FirstName,
                _state.LastName,
                userName),
            _state.UserName, DateTime.UtcNow.ToTimestamp())
            );
    }

    public void UpdateUserPremiumStatus(bool premium)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserPremiumStatusChangedEvent(_state.UserId, _state.PhoneNumber, premium));
    }
    public void UploadProfilePhoto(RequestInfo requestInfo,
        long photoId,
        bool fallback,
        IVideoSize? videoEmojiMarkup
        )
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserProfilePhotoUploadedEvent(requestInfo,
            photoId,
            fallback,
            videoEmojiMarkup,
            DateTime.UtcNow.ToTimestamp()
            /*, hasVideo, videoStartTs*/));
    }

    // Third-Party Verification methods
    public void CreateBotVerifier(long botUserId, long iconEmojiId, string companyName, bool canModifyCustomDescription)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.IsBot)
        {
            RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
        }

        Emit(new BotVerifierCreatedEvent(botUserId, iconEmojiId, companyName, canModifyCustomDescription));
    }

    public void UpdateBotVerifierSettings(long botUserId, long iconEmojiId, string companyName, bool canModifyCustomDescription)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.IsBot)
        {
            RpcErrors.RpcErrors400.BotInvalid.ThrowRpcError();
        }

        Emit(new BotVerifierSettingsUpdatedEvent(botUserId, iconEmojiId, companyName, canModifyCustomDescription));
    }

    public void SetCustomVerification(RequestInfo requestInfo, long targetUserId, long botVerifierId, 
        long iconEmojiId, string description, string? customDescription)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        CheckUserDeletionState();

        Emit(new CustomVerificationSetEvent(requestInfo, targetUserId, botVerifierId, 
            iconEmojiId, description, customDescription));
    }

    public void RemoveCustomVerification(RequestInfo requestInfo, long targetUserId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        CheckUserDeletionState();

        Emit(new CustomVerificationRemovedEvent(requestInfo, targetUserId));
    }

    protected override Task<UserSnapshot> CreateSnapshotAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult(new UserSnapshot(_state.UserId,
            _state.IsOnline,
            _state.AccessHash,
            _state.FirstName,
            _state.LastName,
            _state.PhoneNumber,
            _state.UserName,
            _state.HasPassword,
            _state.Photo,
            _state.IsBot,
            _state.IsDeleted,
            _state.EmojiStatusDocumentId,
            _state.EmojiStatusValidUntil,
            _state.RecentEmojiStatus.ToList(),
            _state.PhotoId,
            _state.FallbackPhotoId,
            _state.Color,
            _state.ProfileColor,
            _state.GlobalPrivacySettings,
            _state.Premium,
            _state.PersonalChannelId,
            _state.Birthday,
            _state.ProfilePhotoUpdateDate,
            _state.UserNameUpdateDate
        ));
    }

    protected override Task LoadSnapshotAsync(UserSnapshot snapshot,
        ISnapshotMetadata metadata,
        CancellationToken cancellationToken)
    {
        _state.LoadFromSnapshot(snapshot);
        return Task.CompletedTask;
    }

    private void CheckUserDeletionState()
    {
        if (_state.IsDeleted)
        {
            RpcErrors.RpcErrors400.PeerIdInvalid.ThrowRpcError();
        }
    }

    public void SetDefaultHistoryTTL(RequestInfo requestInfo, int period)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new UserDefaultHistoryTTLUpdatedEvent(requestInfo, _state.UserId, period));
    }

    public void InstallStickerSet(RequestInfo requestInfo, long stickerSetId, bool archived, StickerSetType stickerSetType)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new StickerSetInstalledEvent(requestInfo, _state.UserId, stickerSetId, archived, stickerSetType));
    }

    public void UpdateBusinessWorkHours(BusinessWorkHours workHours)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessWorkHoursUpdatedEvent(_state.UserId, workHours));
    }

    public void UpdateBusinessLocation(BusinessLocation location)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessLocationUpdatedEvent(_state.UserId, location));
    }

    public void UpdateBusinessGreetingMessage(BusinessGreetingMessage greetingMessage)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessGreetingMessageUpdatedEvent(_state.UserId, greetingMessage));
    }

    public void UpdateBusinessAwayMessage(BusinessAwayMessage awayMessage)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessAwayMessageUpdatedEvent(_state.UserId, awayMessage));
    }

    public void UpdateBusinessIntro(BusinessIntro businessIntro)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessIntroUpdatedEvent(_state.UserId, businessIntro));
    }

    public void CreateBusinessChatLink(BusinessChatLink chatLink)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessChatLinkCreatedEvent(_state.UserId, chatLink));
    }

    public void DeleteBusinessChatLink(string linkId)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        Emit(new BusinessChatLinkDeletedEvent(_state.UserId, linkId));
    }

    // Frozen Account methods
    public void FreezeAccount(
        RequestInfo requestInfo,
        int freezeSinceDate,
        int freezeUntilDate,
        FreezeReason reason,
        string appealUrl,
        long? moderatorUserId,
        string? note)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        // Check if account is already frozen
        if (_state.IsFrozen)
        {
            RpcErrorsCustom.RpcErrors400.UserAlreadyFrozen.ThrowRpcError();
        }
        
        CheckUserDeletionState();
        
        Emit(new UserAccountFrozenEvent(
            requestInfo,
            _state.UserId,
            freezeSinceDate,
            freezeUntilDate,
            reason,
            appealUrl,
            moderatorUserId,
            note
        ));
    }
    
    public void UnfreezeAccount(
        RequestInfo requestInfo,
        UnfreezeReason reason,
        long? moderatorUserId,
        string? note)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.IsFrozen)
        {
            RpcErrorsCustom.RpcErrors400.UserNotFrozen.ThrowRpcError();
        }
        
        Emit(new UserAccountUnfrozenEvent(
            requestInfo,
            _state.UserId,
            reason,
            moderatorUserId,
            note
        ));
    }
    
    public void SubmitFrozenAppeal(
        RequestInfo requestInfo,
        string appealId,
        string appealText,
        string captchaToken,
        string userName,
        Dictionary<string, string> answers)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.IsFrozen)
        {
            RpcErrorsCustom.RpcErrors400.UserNotFrozen.ThrowRpcError();
        }
        
        Emit(new UserFrozenAppealSubmittedEvent(
            requestInfo,
            _state.UserId,
            appealId,
            appealText,
            captchaToken,
            userName,
            answers
        ));
    }
    
    public void ReviewFrozenAppeal(
        RequestInfo requestInfo,
        string appealId,
        AppealStatus status,
        long moderatorUserId,
        string? reviewNote)
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        Emit(new UserFrozenAppealReviewedEvent(
            requestInfo,
            _state.UserId,
            appealId,
            status,
            moderatorUserId,
            reviewNote
        ));
    }
    
    public void AutoDeleteExpiredFrozenAccount()
    {
        Specs.AggregateIsCreated.ThrowDomainErrorIfNotSatisfied(this);
        
        if (!_state.IsFrozen)
        {
            return; // Nothing to do
        }
        
        var now = DateTime.UtcNow.ToTimestamp();
        if (_state.FreezeUntilDate > now)
        {
            return; // Not expired yet
        }
        
        Emit(new UserAccountAutoDeletedEvent(
            _state.UserId,
            _state.FreezeUntilDate!.Value
        ));
    }
}
