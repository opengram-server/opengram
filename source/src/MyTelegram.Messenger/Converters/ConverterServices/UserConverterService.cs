using MyTelegram.Messenger.Services.Impl;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.Business;
using TPeerSettings = MyTelegram.Schema.TPeerSettings;

namespace MyTelegram.Messenger.Converters.ConverterServices;

public class UserConverterService(
    IQueryProcessor queryProcessor,
    IUserAppService userAppService,
    IPhotoAppService photoAppService,
    IPrivacyAppService privacyAppService,
    IPrivacyHelper privacyHelper,
    IContactHelper contactHelper,
    IAccessHashHelper2 accessHashHelper2,
    //IBlockCacheAppService blockCacheAppService,
    IUserStatusCacheAppService userStatusCacheAppService,
    ILayeredService<IUserConverter> userLayeredService,
    ILayeredService<IUserFullConverter> userFullLayeredService,
    ILayeredService<IEmojiStatusConverter> emojiStatusLayeredService,
    ILayeredService<IPhotoConverter> photoLayeredService,
    IFrozenSettingsService frozenSettingsService) : IUserConverterService, ITransientDependency
{
    public async Task<ILayeredUser> GetUserAsync(IRequestWithAccessHashKeyId request, long userId, bool skipSetContactProperties = true,
        bool skipCheckPrivacy = false, int layer = 0)
    {
        var userReadModel = await userAppService.GetAsync(userId);
        if (userReadModel == null)
        {
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }

        //IReadOnlyCollection<IContactReadModel>? contactReadModels = null;
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null;
        IContactReadModel? myContactReadModel = null;
        IContactReadModel? targetUserContactReadModel = null;
        if (!skipSetContactProperties)
        {
            var contactReadModels =
                await queryProcessor.ProcessAsync(new GetContactListBySelfIdAndTargetUserIdQuery(request.UserId, userId));
            myContactReadModel =
                contactReadModels?.FirstOrDefault(p => p.SelfUserId == request.UserId && p.TargetUserId == userId);
            targetUserContactReadModel =
                contactReadModels?.FirstOrDefault(p => p.SelfUserId == userId && p.TargetUserId == request.UserId);
        }
        var photoReadModels = (await photoAppService.GetPhotosAsync(userReadModel, myContactReadModel)).ToDictionary(k => k.PhotoId);

        Console.WriteLine($"[GetUserAsync] RequestUserId={request.UserId}, TargetUserId={userId}, skipCheckPrivacy={skipCheckPrivacy}, skipSetContactProperties={skipSetContactProperties}");
        
        if (!skipCheckPrivacy)
        {
            privacyReadModels = await privacyAppService.GetPrivacyListAsync(userId);
            Console.WriteLine($"[GetUserAsync] Loaded {privacyReadModels?.Count ?? 0} privacy rules for user {userId}");
        }

        return ToUserCore(request, userReadModel, photoReadModels, myContactReadModel, targetUserContactReadModel,
            privacyReadModels, layer);
    }

    public async Task<List<ILayeredUser>> GetUserListAsync(IRequestWithAccessHashKeyId request, List<long> userIds,
        bool skipSetContactProperties = true,
        bool skipCheckPrivacy = false, int layer = 0)
    {
        var userReadModels = await userAppService.GetListAsync(userIds);
        var photoReadModels = await photoAppService.GetPhotosAsync(userReadModels);
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null;
        IReadOnlyCollection<IContactReadModel>? contactReadModels = null;
        if (!skipSetContactProperties)
        {
            contactReadModels = await queryProcessor.ProcessAsync(new GetContactListQuery(request.UserId, userIds));
        }

        if (!skipCheckPrivacy)
        {
            privacyReadModels = await privacyAppService.GetPrivacyListAsync(userIds);
        }

        return ToUserList(request, userReadModels, photoReadModels, contactReadModels, privacyReadModels, layer);
    }

    public IUserFull ToUserFull(IRequestWithAccessHashKeyId request,
        IUserReadModel userReadModel,
        IReadOnlyCollection<IPhotoReadModel>? photoReadModels,
        IReadOnlyCollection<IContactReadModel>? contactReadModels,
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels, int layer = 0)
    {
        var userId = userReadModel.UserId;
        var isOfficialUserId = userId == MyTelegramConsts.OfficialUserId;
        var phoneCallAvailable = !isOfficialUserId &&
                                 !userReadModel.Bot &&
                                 userId != request.UserId;
        var userFull = userFullLayeredService.GetConverter(layer).ToUserFull(userReadModel);
        userFull.CanPinMessage = !isOfficialUserId;

        // Обработка удалённых аккаунтов
        if (userReadModel.IsDeleted == true)
        {
            userFull.Settings = new TPeerSettings
            {
                NeedContactsException = true
            };
            userFull.NotifySettings = new TPeerNotifySettings();

            return userFull;
        }
        
        // Замороженные аккаунты: выводим соответствующий текст в Bio
        if (userReadModel.IsFrozen && request.UserId != userId)
        {
            userFull.About = "Account was frozen";
        }

        userFull.PhoneCallsAvailable = phoneCallAvailable;
        userFull.VideoCallsAvailable = phoneCallAvailable;
        userFull.PhoneCallsPrivate = isOfficialUserId;
        //userFull.Blocked = isBlocked;
        //userFull.NotifySettings

        var photos = photoReadModels?.ToDictionary(k => k.PhotoId) ?? [];

        if (userReadModel.ProfilePhotoId != null)
        {
            if (photos.TryGetValue(userReadModel.ProfilePhotoId.Value, out var profilePhotoReadModel))
            {
                userFull.ProfilePhoto = photoLayeredService.GetConverter(layer).ToPhoto(profilePhotoReadModel);
            }
        }

        if (userReadModel.FallbackPhotoId != null)
        {
            if (photos.TryGetValue(userReadModel.FallbackPhotoId.Value, out var fallbackPhotoReadModel))
            {
                userFull.FallbackPhoto = photoLayeredService.GetConverter(layer).ToPhoto(fallbackPhotoReadModel);
                var profilePhotoId = userReadModel.ProfilePhotoId;

                userFull.ProfilePhoto = profilePhotoId is null or 0 ? null : new TPhotoEmpty { Id = profilePhotoId.Value };
            }
        }

        if (request.UserId != userId)
        {
            var myContactReadModel =
                contactReadModels?.FirstOrDefault(p => p.SelfUserId == request.UserId && p.TargetUserId == userId);
            var targetUserContactReadModel =
                contactReadModels?.FirstOrDefault(p => p.SelfUserId == userId && p.TargetUserId == request.UserId);
            var contactType = contactHelper.GetContactType(myContactReadModel, targetUserContactReadModel);

            ApplyPrivacyToUserFull(request.UserId, userFull, privacyReadModels, contactType);
            ConfigureActionBar(request.UserId, userFull, userReadModel, contactType);

            if (myContactReadModel is { PhotoId: not null })
            {
                if (photos.TryGetValue(myContactReadModel.PhotoId.Value, out var photoReadModel))
                {
                    userFull.PersonalPhoto = photoLayeredService.GetConverter(layer).ToPhoto(photoReadModel);
                    userFull.ProfilePhoto ??= userFull.PersonalPhoto;
                }
            }
        }

        SetBusinessProperties(userFull, userReadModel);

        return userFull;
    }

    private void SetBusinessProperties(IUserFull userFull, IUserReadModel userReadModel)
    {
        if (userReadModel.BusinessWorkHours != null)
        {
            userFull.BusinessWorkHours = new TBusinessWorkHours
            {
                OpenNow = userReadModel.BusinessWorkHours.OpenNow,
                TimezoneId = userReadModel.BusinessWorkHours.TimezoneId,
                WeeklyOpen = new TVector<IBusinessWeeklyOpen>(userReadModel.BusinessWorkHours.WeeklyOpen.Select(p =>
                    new TBusinessWeeklyOpen
                    {
                        StartMinute = p.StartMinute,
                        EndMinute = p.EndMinute
                    }))
            };
        }

        if (userReadModel.BusinessLocation != null)
        {
            userFull.BusinessLocation = new TBusinessLocation
            {
                Address = userReadModel.BusinessLocation.Address,
                GeoPoint = userReadModel.BusinessLocation.Latitude.HasValue &&
                           userReadModel.BusinessLocation.Longitude.HasValue
                    ? new TGeoPoint
                    {
                        Lat = userReadModel.BusinessLocation.Latitude.Value,
                        Long = userReadModel.BusinessLocation.Longitude.Value,
                        AccessHash = 0 // для публичных локаций access hash обычно равен 0 либо вычисляется
                    }
                    : null
            };
        }

        if (userReadModel.BusinessGreetingMessage != null)
        {
            userFull.BusinessGreetingMessage = new TBusinessGreetingMessage
            {
                ShortcutId = userReadModel.BusinessGreetingMessage.ShortcutId,
                Recipients = MapBusinessRecipients(userReadModel.BusinessGreetingMessage.Recipients),
                NoActivityDays = userReadModel.BusinessGreetingMessage.NoActivityDays
            };
        }

        if (userReadModel.BusinessAwayMessage != null)
        {
            IBusinessAwayMessageSchedule schedule = userReadModel.BusinessAwayMessage.Schedule.Type switch
            {
                BusinessAwayMessageScheduleType.Always => new TBusinessAwayMessageScheduleAlways(),
                BusinessAwayMessageScheduleType.OutsideWorkHours => new TBusinessAwayMessageScheduleOutsideWorkHours(),
                BusinessAwayMessageScheduleType.Custom => new TBusinessAwayMessageScheduleCustom
                {
                    StartDate = userReadModel.BusinessAwayMessage.Schedule.StartMinute ?? 0, // в Domain хранится StartMinute, а Schema, похоже, ожидает Date/Timestamp — требуется проверка
                    EndDate = userReadModel.BusinessAwayMessage.Schedule.EndMinute ?? 0
                },
                _ => new TBusinessAwayMessageScheduleAlways()
            };

            userFull.BusinessAwayMessage = new TBusinessAwayMessage
            {
                ShortcutId = userReadModel.BusinessAwayMessage.ShortcutId,
                Recipients = MapBusinessRecipients(userReadModel.BusinessAwayMessage.Recipients),
                Schedule = schedule,
                OfflineOnly = userReadModel.BusinessAwayMessage.OfflineOnly
            };
        }

        if (userReadModel.BusinessIntro != null)
        {
            // StickerDocumentId по возможности нужно превратить в Document,
            // но здесь у нас может быть только идентификатор.
            // TBusinessIntro ожидает поле sticker типа IDocument, поэтому документ
            // пришлось бы загружать отдельно либо подготавливать заранее.
            // Сейчас сделать это без асинхронной загрузки или предзагрузки нельзя,
            // поэтому при наличии StickerDocumentId оставляем поле пустым.
            // TODO: предзагружать стикер Business Intro в GetFullUserHandler

            userFull.BusinessIntro = new TBusinessIntro
            {
                Title = userReadModel.BusinessIntro.Title,
                Description = userReadModel.BusinessIntro.Description,
                Sticker = null // заглушка, пока не реализована загрузка документа
            };
        }
    }

    private IBusinessRecipients MapBusinessRecipients(MyTelegram.Domain.Shared.Business.BusinessRecipients recipients)
    {
        return new TBusinessRecipients
        {
            ExistingChats = recipients.ExistingChats,
            NewChats = recipients.NewChats,
            Contacts = recipients.Contacts,
            NonContacts = recipients.NonContacts,
            ExcludeSelected = recipients.ExcludeSelected,
            Users = new TVector<long>(recipients.Users)
        };
    }

    public async Task<IUserFull> GetUserFullAsync(IRequestWithAccessHashKeyId request, long userId, int layer = 0)
    {
        var userReadModel = await userAppService.GetAsync(userId);
        //var isBlocked = await blockCacheAppService.IsBlockedAsync(selfUserId, userId);
        var privacyReadModels = await privacyAppService.GetPrivacyListAsync(userId);
        IReadOnlyCollection<IContactReadModel>? contactReadModels = null;
        IContactReadModel? myContactReadModel = null;
        if (request.UserId != userId)
        {
            contactReadModels =
              await queryProcessor.ProcessAsync(new GetContactListBySelfIdAndTargetUserIdQuery(request.UserId, userId));
            myContactReadModel =
                contactReadModels?.FirstOrDefault(p => p.SelfUserId == request.UserId && p.TargetUserId == userId);
        }
        var photoReadModels = await photoAppService.GetPhotosAsync(userReadModel, myContactReadModel);

        var userFull = ToUserFull(request, userReadModel, photoReadModels, contactReadModels, privacyReadModels, layer);

        // Добавляем информацию о верификации бота
        if (userReadModel.BotVerifierId.HasValue)
        {
            var verification = await queryProcessor.ProcessAsync(
                new GetBotVerificationByTargetQuery(VerificationTargetType.User, userId), 
                default);
            
            if (verification != null)
            {
                userFull.BotVerification = new TBotVerification
                {
                    BotId = verification.BotVerifierId,
                    Icon = verification.IconEmojiId,
                    Description = verification.CustomDescription ?? verification.Description
                };
            }
        }

        return userFull;
    }

    public ILayeredUser ToUser(IRequestWithAccessHashKeyId request, IUserReadModel userReadModel, IReadOnlyCollection<IPhotoReadModel>? photoReadModels = null,
        IContactReadModel? contactReadModel = null, IContactReadModel? targetUserContactReadModel = null, IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null, int layer = 0)
    {
        var photos = photoReadModels?.ToDictionary(k => k.PhotoId);

        return ToUserCore(request, userReadModel, photos, contactReadModel, targetUserContactReadModel,
            privacyReadModels, layer);
    }

    public List<ILayeredUser> ToUserList(IRequestWithAccessHashKeyId request, IReadOnlyCollection<IUserReadModel> userReadModels, IReadOnlyCollection<IPhotoReadModel>? photoReadModels = null,
        IReadOnlyCollection<IContactReadModel>? contactReadModels = null, IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null, int layer = 0)
    {
        var users = new List<ILayeredUser>();

        var photos = photoReadModels?
            .DistinctBy(p => p.PhotoId)
            .ToDictionary(k => k.PhotoId) ?? [];

        var targetUserContacts = contactReadModels?
             .DistinctBy(p => p.SelfUserId)
             .ToDictionary(k => k.SelfUserId) ?? [];

        var myContacts = contactReadModels?
             .Where(p => p.SelfUserId == request.UserId)
             .DistinctBy(p => p.TargetUserId)
             .ToDictionary(k => k.TargetUserId) ?? [];

        var groupedPrivacyReadModels = privacyReadModels?.GroupBy(p => p.UserId).ToDictionary(k => k.Key, v => v.ToList()) ?? [];

        foreach (var userReadModel in userReadModels)
        {
            myContacts.TryGetValue(userReadModel.UserId, out var myContactReadModel);
            targetUserContacts.TryGetValue(request.UserId, out var targetUserContactReadModel);
            groupedPrivacyReadModels.TryGetValue(userReadModel.UserId, out var currentUserPrivacyReadModels);
            var user = ToUserCore(request, userReadModel, photos, myContactReadModel,
                targetUserContactReadModel, currentUserPrivacyReadModels, layer);
            users.Add(user);
        }

        return users;
    }

    private ILayeredUser ToUserCore(IRequestWithAccessHashKeyId request, IUserReadModel userReadModel,
        Dictionary<long, IPhotoReadModel>? photoReadModels = null,
        //Dictionary<long, IContactReadModel>? contactReadModels = null,
        IContactReadModel? myContactReadModel = null,
        IContactReadModel? targetUserContactReadModel = null,
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels = null, int layer = 0)
    {
        var user = userLayeredService.GetConverter(layer).ToUser(userReadModel);
        
        // Сразу скрываем телефон для чужих пользователей.
        // Так Вы исключаете момент, когда телефон ещё виден до вызова ApplyPrivacyToUser.
        if (request.UserId != userReadModel.UserId)
        {
            user.Phone = null; // по умолчанию скрыт, восстановим позже, если это контакт
            Console.WriteLine($"[PRIVACY] Pre-hiding phone for user {userReadModel.UserId} (will check contact status later)");
        }
        
        user.AccessHash = 0;
        if (request.AccessHashKeyId != 0)
        {
            user.AccessHash = accessHashHelper2.GenerateAccessHash(request.UserId, request.AccessHashKeyId,
                userReadModel.UserId, AccessHashType.User);
        }

        // Обработка удалённых или замороженных аккаунтов
        if (userReadModel.IsDeleted == true)
        {
            user.Deleted = true;
            user.Photo = null;

            return user;
        }
        
        Console.WriteLine($"ToUser: UserId={userReadModel.UserId}, IsFrozen={userReadModel.IsFrozen}, RequestUserId={request.UserId}");

        // Замороженные аккаунты помечаем как ограниченные (а не удалённые), чтобы остался эмодзи-статус «снежинка»
        if (userReadModel.IsFrozen && request.UserId != userReadModel.UserId)
        {
            // Не выставляем Deleted = true, так как это скрывает EmojiStatus.
            // Вместо этого используем флаг Restricted, при котором эмодзи-статус остаётся видимым.
            user.Photo = null;

            if (user is TUser frozenTUser)
            {
                frozenTUser.Premium = false;

                // Отмечаем как ограниченный, чтобы эмодзи-статус всё равно показывался
                frozenTUser.Restricted = true;

                // Выставляем эмодзи-статус «снежинка»
                var snowflakeEmojiId = frozenSettingsService.GetSnowflakeEmojiIdAsync().GetAwaiter().GetResult();
                if (snowflakeEmojiId.HasValue && snowflakeEmojiId.Value > 0)
                {
                    frozenTUser.EmojiStatus = new TEmojiStatus
                    {
                        DocumentId = snowflakeEmojiId.Value
                    };
                }
            }
        }

        if (request.UserId == userReadModel.UserId)
        {
            user.Self = true;
        }

        user.Status = userStatusCacheAppService.GetUserStatus(user.Id);
        
        EmojiStatus? emojiStatus = null;
        if (userReadModel.EmojiStatusDocumentId != null)
        {
            emojiStatus = new EmojiStatus(userReadModel.EmojiStatusDocumentId.Value, userReadModel.EmojiStatusValidUntil);
        }

        user.EmojiStatus = emojiStatusLayeredService.GetConverter(layer).ToEmojiStatus(emojiStatus);
        
        // Выставляем иконку верификации бота и статус Premium, если у пользователя есть сторонняя верификация
        if (user is TUser tUser)
        {
            // Берём статус Premium из UserReadModel.
            // Это перекрывает значение из UserMapper, чтобы Premium всегда был актуальным.
            var wasPremium = tUser.Premium;
            tUser.Premium = userReadModel.Premium;
            
            if (userReadModel.BotVerificationIcon.HasValue)
            {
                tUser.BotVerificationIcon = userReadModel.BotVerificationIcon.Value;
            }
        }
        else
        {
            // Сюда не должны попадать: объект user не является TUser
            Console.WriteLine($"*** [UserConverterService] WARNING: User {userReadModel.UserId} is not TUser, it's {user.GetType().Name} - Premium cannot be set! ***");
        }
        
        //var contactReadModel =
        //    contactReadModels?.FirstOrDefault(p =>
        //        p.SelfUserId == selfUserId && p.TargetUserId == userReadModel.UserId);
        var contactType = contactHelper.GetContactType(myContactReadModel, targetUserContactReadModel);
        var photos = photoReadModels ?? [];
        SetUserProfilePhoto(userReadModel, user, photos, layer);

        // Применяем настройки приватности, чтобы скрыть телефон/фото/статус согласно правилам пользователя
        if (privacyReadModels != null && privacyReadModels.Count > 0)
        {
            ApplyPrivacyToUser(request.UserId, userReadModel, user, photos, contactType, privacyReadModels, layer);
        }

        return user;
    }

    private void ApplyPrivacyToUserFull(long selfUserId,
        IUserFull userFull,
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels,
        MyTelegram.ContactType contactType)
    {
        if (selfUserId == userFull.Id)
        {
            return;
        }

        // Поведение Telegram по умолчанию для полей UserFull, когда правила приватности не заданы
        var isContact = contactType is MyTelegram.ContactType.TargetUserIsMyContact or MyTelegram.ContactType.Mutual;

        // About по умолчанию виден всем (если нет отдельного правила)
        var hasAboutRule = privacyReadModels?.Any(p => p.PrivacyType == PrivacyType.About) ?? false;
        if (!hasAboutRule)
        {
            // оставляем About видимым (значение по умолчанию)
        }

        // Birthday по умолчанию виден всем (если нет отдельного правила)
        var hasBirthdayRule = privacyReadModels?.Any(p => p.PrivacyType == PrivacyType.Birthday) ?? false;
        if (!hasBirthdayRule)
        {
            // оставляем Birthday видимым (значение по умолчанию)
        }

        foreach (var privacy in privacyReadModels ?? [])
        {
            switch (privacy.PrivacyType)
            {
                case PrivacyType.PhoneCall:
                    privacyHelper.ApplyPrivacy(privacy, _ =>
                    {
                        userFull.PhoneCallsAvailable = false;
                        userFull.PhoneCallsPrivate = false;
                    }, selfUserId, contactType);
                    break;

                case PrivacyType.ProfilePhoto:
                    privacyHelper.ApplyPrivacy(privacy,
                        _ =>
                        {
                            userFull.ProfilePhoto = null;
                        },
                        selfUserId, contactType);
                    break;

                case PrivacyType.VoiceMessages:
                    privacyHelper.ApplyPrivacy(privacy, _ => { userFull.VoiceMessagesForbidden = true; },
                        selfUserId, contactType);
                    break;
                case PrivacyType.About:
                    privacyHelper.ApplyPrivacy(privacy, _ => { userFull.About = null; }, selfUserId, contactType);
                    break;

                case PrivacyType.Birthday:
                    privacyHelper.ApplyPrivacy(privacy, _ =>
                    {
                        userFull.Birthday = null;
                    }, selfUserId, contactType);
                    break;
            }
        }
    }

    private void ConfigureActionBar(long selfUserId, IUserFull userFull, IUserReadModel userReadModel, MyTelegram.ContactType contactType)
    {
        var settings = userFull.Settings as TPeerSettings ?? new TPeerSettings();
        
        // Проверяем, не заморожен ли (ограничен) аккаунт
        if (userReadModel.Restricted)
        {
            Console.WriteLine($"*** FROZEN ACCOUNT DETECTED: UserId={userReadModel.UserId}, RestrictionReason={userReadModel.RestrictionReason} ***");

            // Для замороженного аккаунта показываем предупреждение в action bar.
            // Клиент покажет причину ограничения и ограничит взаимодействие.
            settings.ReportSpam = true;
            settings.BlockContact = true;
            // Для ограниченных аккаунтов кнопку AddContact не показываем

            // Причина ограничения добавляется в UserMapper.cs.
            // Объект User будет содержать причину ограничения с иконкой-снежинкой.

            userFull.Settings = settings;
            return;
        }

        // Определяем, является ли пользователь незнакомым (не в контактах).
        // Action bar показываем при: None (нет связи) и ContactOfTargetUser (он добавил нас, мы его — нет).
        var isUnknownUser = contactType == MyTelegram.ContactType.None ||
                           contactType == MyTelegram.ContactType.ContactOfTargetUser;

        if (isUnknownUser)
        {
            // Показываем action bar с кнопками Report Spam, Add Contact, Block
            settings.ReportSpam = true;
            settings.AddContact = true;
            settings.BlockContact = true;

            // Месяц регистрации в формате MM.YYYY (например, "01.2024" для января 2024).
            // Помогает заметить недавно созданные аккаунты (возможный спам).
            if (userReadModel.CreationTime.HasValue)
            {
                var creationTime = userReadModel.CreationTime.Value;
                settings.RegistrationMonth = $"{creationTime.Month:D2}.{creationTime.Year}";
            }

            // Код страны телефона (например, "US", "RU", "GB").
            // Помогает понять происхождение аккаунта.
            settings.PhoneCountry = userReadModel.PhoneCountryCode;

            // Unix-время последней смены имени пользователем.
            // Частые смены имени могут указывать на подозрительную активность.
            if (userReadModel.UserNameUpdateDate.HasValue)
            {
                settings.NameChangeDate = userReadModel.UserNameUpdateDate.Value;
            }

            // Unix-время последней смены фото профиля.
            // Помогает выявлять угнанные аккаунты.
            if (userReadModel.ProfilePhotoUpdateDate.HasValue)
            {
                settings.PhotoChangeDate = userReadModel.ProfilePhotoUpdateDate.Value;
            }
        }
        else if (contactType == MyTelegram.ContactType.TargetUserIsMyContact ||
                 contactType == MyTelegram.ContactType.Mutual)
        {
            // Известный контакт — показываем минимальный action bar
            settings.ShareContact = true;
            settings.BlockContact = true;
        }
        
        userFull.Settings = settings;
    }

    private void ApplyPrivacyToUser(long selfUserId, IUserReadModel userReadModel, ILayeredUser user,
        Dictionary<long, IPhotoReadModel> photos, MyTelegram.ContactType schemaContactType,
        IReadOnlyCollection<IPrivacyReadModel>? privacyReadModels, int layer)
    {
        // Преобразуем ContactType из схемы обратно в доменный для проверок приватности
        var contactType = schemaContactType switch
        {
            MyTelegram.ContactType.Self => MyTelegram.ContactType.Self,
            MyTelegram.ContactType.TargetUserIsMyContact => MyTelegram.ContactType.TargetUserIsMyContact,
            MyTelegram.ContactType.Mutual => MyTelegram.ContactType.Mutual,
            MyTelegram.ContactType.TargetUserIsNotMyContact => MyTelegram.ContactType.TargetUserIsNotMyContact,
            _ => MyTelegram.ContactType.None
        };
        
        //var privacyReadModels = await privacyAppService.GetPrivacyListAsync(userReadModel.UserId);
        if (selfUserId != userReadModel.UserId)
        {
            // Логика приватности телефона.
            // Стандартное поведение Telegram: для не-контактов телефон скрыт (независимо от правил приватности).
            var isContact = contactType is MyTelegram.ContactType.TargetUserIsMyContact or MyTelegram.ContactType.Mutual;
            var hasPhonePrivacyRule = privacyReadModels?.Any(p => p.PrivacyType == PrivacyType.PhoneNumber) ?? false;

            Console.WriteLine($"[PHONE PRIVACY] Self={selfUserId}, Target={userReadModel.UserId}, Contact={isContact}, HasRule={hasPhonePrivacyRule}, Phone={user.Phone}");

            // Восстанавливаем телефон только для контактов (в ToUser он по умолчанию скрыт).
            // Правила приватности могут лишь сильнее ограничить доступ, но не-контакты телефон видеть не должны.
            if (isContact)
            {
                // Это контакт — возвращаем номер телефона
                user.Phone = userReadModel.PhoneNumber;
                Console.WriteLine($"[PHONE PRIVACY] RESTORING phone for user {userReadModel.UserId} (is a contact)");
            }
            else
            {
                // Не контакт — оставляем телефон скрытым (уже null после ToUser)
                Console.WriteLine($"[PHONE PRIVACY] KEEPING phone hidden for user {userReadModel.UserId} (not a contact)");
            }
            
            if (privacyReadModels?.Count > 0)
            {
                photos.TryGetValue(userReadModel.FallbackPhotoId ?? 0, out var fallbackPhotoReadModel);
                //var contactType = contactHelper.GetContactType(selfUserId, userReadModel.UserId, contactReadModels);

                foreach (var privacy in privacyReadModels)
                {
                    switch (privacy.PrivacyType)
                    {
                        case PrivacyType.StatusTimestamp:
                        privacyHelper.ApplyPrivacy(privacy,
                            _ =>
                            {
                                // При включённой приватности скрываем точное время и показываем "recently"
                                switch (user.Status)
                                {
                                    case TUserStatusOnline userStatusOnline:
                                        // Онлайн-статус можно скрыть согласно правилам приватности
                                        user.Status = new TUserStatusRecently();
                                        break;
                                    case TUserStatusOffline userStatusOffline:
                                        // Скрываем точное время выхода, показываем "recently"
                                        user.Status = new TUserStatusRecently();
                                        break;
                                    case TUserStatusRecently:
                                        // Уже показывается "recently", оставляем как есть
                                        break;
                                    case TUserStatusLastWeek:
                                        // Оставляем приблизительный статус
                                        break;
                                    case TUserStatusLastMonth:
                                        // Оставляем приблизительный статус
                                        break;
                                    default:
                                        user.Status = new TUserStatusRecently();
                                        break;
                                }
                            },
                            selfUserId,
                            contactType);
                        break;
                    case PrivacyType.ProfilePhoto:
                        privacyHelper.ApplyPrivacy(privacy,
                            _ => user.Photo = photoLayeredService.GetConverter(layer)
                                .ToProfilePhoto(fallbackPhotoReadModel), selfUserId,
                            contactType);
                        break;
                    case PrivacyType.PhoneNumber:
                        privacyHelper.ApplyPrivacy(privacy, _ => user.Phone = null, selfUserId, contactType);
                        break;
                    }
                }
            }
        }
    }

    private void SetContactPersonalProfilePhoto(ILayeredUser user, Dictionary<long, IPhotoReadModel> photos,
        IContactReadModel? contactReadModel,
        int layer)
    {
        if (contactReadModel != null)
        {
            user.Contact = true;
            user.FirstName = contactReadModel.FirstName;
            user.LastName = contactReadModel.LastName;

            if (contactReadModel.PhotoId != null)
            {
                if (photos.TryGetValue(contactReadModel.PhotoId.Value, out var photoReadModel))
                {
                    user.Photo = photoLayeredService.GetConverter(layer).ToProfilePhoto(photoReadModel);
                    if (user.Photo is TUserProfilePhoto profilePhoto)
                    {
                        profilePhoto.Personal = true;
                    }
                }
            }
        }
    }

    private void SetUserProfilePhoto(IUserReadModel userReadModel,
        ILayeredUser user, Dictionary<long, IPhotoReadModel> photoReadModels, int layer)
    {
        if (userReadModel.ProfilePhotoId != null)
        {
            if (photoReadModels.TryGetValue(userReadModel.ProfilePhotoId.Value, out var photoReadModel))
            {
                user.Photo = photoLayeredService.GetConverter(layer).ToProfilePhoto(photoReadModel);
            }
        }
    }
}