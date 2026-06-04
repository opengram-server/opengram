namespace MyTelegram.Converters.Mappers.LatestLayer;

internal sealed class UserMapper
    : IObjectMapper<IUserReadModel, TUser>,
        ILayeredMapper,
        ITransientDependency
{
    public int Layer => Layers.LayerLatest;
    

    public TUser Map(IUserReadModel source)
    {
        return Map(source, new TUser());
    }

    public TUser Map(
        IUserReadModel source,
        TUser destination
    )
    {
        // Логируем создание объекта User
        if (source.BotVerificationIcon.HasValue || source.EmojiStatusDocumentId.HasValue)
        {
            Console.WriteLine($"*** UserMapper.Map: User {source.UserId} - BotVerificationIcon={source.BotVerificationIcon}, EmojiStatus={source.EmojiStatusDocumentId} ***");
        }
        //destination.Self = source.Self;
        //destination.Contact = source.Contact;
        //destination.MutualContact = source.MutualContact;
        //destination.Deleted = source.Deleted;
        //destination.Bot = source.Bot;
        //destination.BotChatHistory = source.BotChatHistory;
        //destination.BotNochats = source.BotNochats;
        //destination.Verified = source.Verified;
        //destination.Restricted = source.Restricted;
        //destination.Min = source.Min;
        //destination.BotInlineGeo = source.BotInlineGeo;
        //destination.Support = source.Support;
        //destination.Scam = source.Scam;
        //destination.ApplyMinPhoto = source.ApplyMinPhoto;
        //destination.Fake = source.Fake;
        //destination.BotAttachMenu = source.BotAttachMenu;
        //destination.Premium = source.Premium;
        //destination.AttachMenuEnabled = source.AttachMenuEnabled;
        //destination.BotCanEdit = source.BotCanEdit;
        //destination.CloseFriend = source.CloseFriend;
        //destination.StoriesHidden = source.StoriesHidden;
        //destination.StoriesUnavailable = source.StoriesUnavailable;
        //destination.ContactRequirePremium = source.ContactRequirePremium;
        //destination.BotBusiness = source.BotBusiness;
        //destination.BotHasMainApp = source.BotHasMainApp;
        //destination.Id = source.Id;
        //destination.AccessHash = source.AccessHash;
        //destination.FirstName = source.FirstName;
        //destination.LastName = source.LastName;
        //destination.Username = source.Username;
        //destination.Phone = source.Phone;
        //destination.Photo = source.Photo;
        //destination.Status = source.Status;
        //destination.BotInfoVersion = source.BotInfoVersion;
        //destination.RestrictionReason = source.RestrictionReason;
        //destination.BotInlinePlaceholder = source.BotInlinePlaceholder;
        //destination.LangCode = source.LangCode;
        //destination.EmojiStatus = source.EmojiStatus;
        //destination.Usernames = source.Usernames;
        //destination.StoriesMaxId = source.StoriesMaxId;
        //destination.Color = source.Color;
        //destination.ProfileColor = source.ProfileColor;
        //destination.BotActiveUsers = source.BotActiveUsers;
        //destination.BotVerificationIcon = source.BotVerificationIcon;

        //return destination;

        destination.Id = source.UserId;
        destination.Photo = new TUserProfilePhotoEmpty();
        destination.AccessHash = source.AccessHash;
        destination.Bot = source.Bot;
        destination.BotInfoVersion = source.BotInfoVersion;
        destination.Username = source.UserName;
        destination.Phone = source.PhoneNumber;
        
        // Добавляем к имени аккаунта иконку заморозки.
        // Иконка настраивается через админ-панель (Settings -> Frozen Icon).
        // По умолчанию используется снежинка.
        // Администратор может загрузить свою JSON-анимацию или сменить эмодзи.
        if (source.Restricted && source.FrozenAnimationDocumentId.HasValue)
        {
            // Для замороженных аккаунтов используем JSON-анимацию как эмодзи-статус
            destination.EmojiStatus = new TEmojiStatus
            {
                DocumentId = source.FrozenAnimationDocumentId.Value,
                Until = null // постоянная анимация заморозки
            };
            destination.FirstName = $"🚫 {source.FirstName}"; // запасная иконка
        }
        else if (source.Restricted)
        {
            destination.FirstName = $"❄️ {source.FirstName}"; // снежинка по умолчанию
        }
        else
        {
            destination.FirstName = source.FirstName;
        }
        destination.LastName = source.LastName;
        
        destination.Verified = source.Verified;
        destination.Restricted = source.Restricted;
        destination.Support = source.Support;
        
        // Снимаем Premium у удалённых/ограниченных аккаунтов
        destination.Premium = (source.IsDeleted == true || source.Restricted) ? false : source.Premium;

        // Выставляем эмодзи-статус: анимация заморозки имеет приоритет над обычным статусом
        if (source.Restricted && source.FrozenAnimationDocumentId.HasValue)
        {
            // анимация заморозки уже выставлена выше
        }
        else if (source.EmojiStatusCollectibleId.HasValue)
        {
            // Для улучшенных звёздных подарков используем TEmojiStatusCollectible
            destination.EmojiStatus = new TEmojiStatusCollectible
            {
                DocumentId = source.EmojiStatusDocumentId ?? 0,
                Until = source.EmojiStatusValidUntil,
                CollectibleId = source.EmojiStatusCollectibleId.Value
            };
            Console.WriteLine($"[UserMapper] User {source.UserId} has collectible emoji status: DocumentId={source.EmojiStatusDocumentId}, CollectibleId={source.EmojiStatusCollectibleId}");
        }
        else if (source.EmojiStatusDocumentId.HasValue)
        {
            destination.EmojiStatus = new TEmojiStatus
            {
                DocumentId = source.EmojiStatusDocumentId.Value,
                Until = source.EmojiStatusValidUntil
            };
        }

        // Добавляем причину ограничения, если аккаунт заморожен
        if (source.Restricted && !string.IsNullOrEmpty(source.RestrictionReason))
        {
            destination.RestrictionReason = new TVector<IRestrictionReason>
            {
                new TRestrictionReason
                {
                    Platform = "all",
                    Reason = "spam", // также возможны "copyright", "pornography" и т.д.
                    Text = $"❄️ {source.RestrictionReason}" // иконка-снежинка для замороженных аккаунтов
                }
            };
        }

        destination.Color = source.Color.ToPeerColor();
        destination.ProfileColor = source.ProfileColor.ToPeerColor();
        destination.ContactRequirePremium = source.GlobalPrivacySettings?.NewNoncontactPeersRequirePremium ?? false;
        destination.BotHasMainApp = source.BotHasMainApp;
        destination.BotActiveUsers = source.BotActiveUsers;
        
        // Иконка верификации бота временно отключена.
        // BotVerificationIcon должен быть SVG document ID, а не emoji document ID.
        // TODO: реализовать загрузку SVG verification icons через админку.
        // У удалённых/ограниченных аккаунтов верификацию бота убираем.
        if (source.IsDeleted == true || source.Restricted)
        {
            destination.BotVerificationIcon = null;
        }
        // destination.BotVerificationIcon = source.BotVerificationIcon;
        
        // Логируем статус верификации для отладки
        if (source.BotVerificationIcon.HasValue && source.IsDeleted != true && !source.Restricted)
        {
            Console.WriteLine($"*** UserMapper: User {source.UserId} has BotVerificationIcon={source.BotVerificationIcon.Value} (DISABLED - needs SVG document), BotVerifierId={source.BotVerifierId} ***");
        }

        if (source.Usernames?.Count > 0)
        {
            destination.Usernames = [];
            if (!string.IsNullOrEmpty(destination.Username))
            {
                destination.Usernames.Add(new TUsername
                {
                    Active = true,
                    Editable = true,
                    Username = destination.Username
                });
                destination.Username = null;
            }

            foreach (var username in source.Usernames)
            {
                destination.Usernames.Add(new TUsername
                {
                    Active = true,
                    Editable = false,
                    Username = username
                });
            }
        }

        return destination;
    }
}