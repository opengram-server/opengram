using MyTelegram.Messenger.Services.Impl;
using TUserFull = MyTelegram.Schema.Users.TUserFull;

namespace MyTelegram.Messenger.Handlers.LatestLayer.Impl.Users;

///<summary>
/// Returns extended user info by ID.
/// <para>Possible errors</para>
/// Code Type Description
/// 400 CHANNEL_INVALID The provided channel is invalid.
/// 400 CHANNEL_PRIVATE You haven't joined this channel/supergroup.
/// 400 MSG_ID_INVALID Invalid message ID provided.
/// 400 USERNAME_OCCUPIED The provided username is already occupied.
/// 400 USER_ID_INVALID The provided user ID is invalid.
/// See <a href="https://corefork.telegram.org/method/users.getFullUser" />
///</summary>
internal sealed class GetFullUserHandler(
    IPeerHelper peerHelper,
    IQueryProcessor queryProcessor,
    IUserConverterService userConverterService,
    ILayeredService<IPeerSettingsConverter> peerSettingsLayeredService,
    ILayeredService<IPeerNotifySettingsConverter> peerNotifySettingsLayeredService,
    IBlockCacheAppService blockCacheAppService,
    IAccessHashHelper accessHashHelper,
    IContactHelper contactHelper,
    IPeerSettingsAppService peerSettingsAppService,
    IPhotoAppService photoAppService,
    IUserAppService userAppService,
    IPrivacyAppService privacyAppService,
    IStickerAppService stickerAppService,
    ILogger<GetFullUserHandler> logger)
    : RpcResultObjectHandler<MyTelegram.Schema.Users.RequestGetFullUser,
            MyTelegram.Schema.Users.IUserFull>,
        Users.IGetFullUserHandler
{
    protected override async Task<MyTelegram.Schema.Users.IUserFull> HandleCoreAsync(IRequestInput input,
        MyTelegram.Schema.Users.RequestGetFullUser obj)
    {
        await accessHashHelper.CheckAccessHashAsync(input, obj.Id);

        var selfUserId = input.UserId;
        var targetPeer = peerHelper.GetPeer(obj.Id, input.UserId);
        var targetUserId = targetPeer.PeerId;

        var userReadModel = await userAppService.GetAsync(targetPeer.PeerId);
        if (userReadModel == null)
        {
            //RpcErrors.RpcErrors400.UserIdInvalid.ThrowRpcError();
            throw new RpcException(RpcErrors.RpcErrors400.UserIdInvalid);
        }
        
        // Завершаем сессию для удалённых или ограниченных пользователей
        if (targetUserId == selfUserId && (userReadModel.IsDeleted == true || userReadModel.Restricted))
        {
            logger.LogWarning("*** User {UserId} is deleted/restricted - kicking from session ***", targetUserId);
            throw new RpcException(new RpcError(401, "AUTH_USER_DELETED"));
        }

        var photoReadModels = await photoAppService.GetPhotosAsync(userReadModel);
        var privacyReadModels = await privacyAppService.GetPrivacyListAsync(targetUserId);
        var contactReadModels =
            await queryProcessor.ProcessAsync(new GetContactListBySelfIdAndTargetUserIdQuery(input.UserId, targetUserId));

        var myContactReadModel =
              contactReadModels?.FirstOrDefault(p => p.SelfUserId == selfUserId && p.TargetUserId == targetUserId);
        var targetUserContactReadModel =
              contactReadModels?.FirstOrDefault(p => p.SelfUserId == targetUserId && p.TargetUserId == selfUserId);

        var peerNotifySettingsId = MyTelegram.Domain.Aggregates.PeerNotifySetting.PeerNotifySettingsId.Create(selfUserId, targetPeer.PeerType, targetPeer.PeerId);
        var peerNotifySettingReadModel =
            await queryProcessor.ProcessAsync(new MyTelegram.Queries.GetPeerNotifySettingsByIdQuery(peerNotifySettingsId.Value));
        var peerSettingReadModel = await peerSettingsAppService.GetPeerSettingsAsync(input.UserId, targetPeer.PeerId);
        var contactType = contactHelper.GetContactType(myContactReadModel, targetUserContactReadModel);// await contactAppService.GetContactTypeAsync(input.UserId, targetPeer.PeerId);
        
        // Преобразуем доменный ContactType в ContactType схемы для peer settings
        var schemaContactType = contactType switch
        {
            MyTelegram.ContactType.Self => MyTelegram.ContactType.Self,
            MyTelegram.ContactType.TargetUserIsMyContact => MyTelegram.ContactType.TargetUserIsMyContact,
            MyTelegram.ContactType.Mutual => MyTelegram.ContactType.Mutual,
            MyTelegram.ContactType.TargetUserIsNotMyContact => MyTelegram.ContactType.TargetUserIsNotMyContact,
            _ => MyTelegram.ContactType.None
        };
        
        var peerSettings = peerSettingsLayeredService.GetConverter(input.Layer).ToPeerSettings(input.UserId,
            targetPeer.PeerId,
            peerSettingReadModel,
            schemaContactType
        );
        var peerNotifySettings = peerNotifySettingsLayeredService.GetConverter(input.Layer)
            .ToPeerNotifySettings(peerNotifySettingReadModel?.NotifySettings ?? PeerNotifySettings.DefaultSettings);

        var userFull = userConverterService.ToUserFull(input, userReadModel, photoReadModels, contactReadModels,
            privacyReadModels, input.Layer);

        userFull.Settings = peerSettings;
        userFull.NotifySettings = peerNotifySettings;
        userFull.Blocked = await blockCacheAppService.IsBlockedAsync(input.UserId, targetPeer.PeerId);
        userFull.TtlPeriod = userReadModel.DefaultHistoryTTL;
        
        // Проверяем, есть ли у пользователя сохранённые звёздные подарки, чтобы показать кнопку
        var giftsCount = await GetStarGiftsCountAsync(targetPeer.PeerId);
        var hasGifts = giftsCount > 0;


        // Включаем кнопку подарков и задаём их количество
        userFull.DisplayGiftsButton = hasGifts;
        userFull.StargiftsCount = giftsCount > 0 ? giftsCount : null; // Задаём количество подарков
        userFull.DisallowedGifts = null; // NULL заставляет клиент использовать DisplayGiftsButton для флага SendHide

        var user = userConverterService.ToUser(input, userReadModel, photoReadModels, myContactReadModel,
            targetUserContactReadModel, privacyReadModels, input.Layer);

        await SetPersonalChannelAsync(input, userReadModel, userFull);
        await SetCommonChatCountAsync(input, userReadModel, userFull);
        await SetBotVerificationAsync(input, userReadModel, userFull);
        
        // Set saved music (Layer 213)
        await SetSavedMusicAsync(input, userReadModel, userFull);
        
        // Предзагружаем документ emoji-статуса, если он есть
        if (userReadModel.EmojiStatusDocumentId.HasValue)
        {
            // Инициируем загрузку документа запросом к нему,
            // чтобы он закэшировался на стороне клиента
            try
            {
                var emojiDoc = await queryProcessor.ProcessAsync(
                    new GetDocumentByIdQuery(userReadModel.EmojiStatusDocumentId.Value));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to preload emoji document {DocumentId}", 
                    userReadModel.EmojiStatusDocumentId.Value);
            }
        }
        
        // Загружаем и проставляем business intro, если он есть
        await SetBusinessIntroAsync(input, userReadModel, userFull);

        // Флаги вычисляются автоматически

        var result = new TUserFull
        {
            Chats = [],
            FullUser = userFull,
            Users = new TVector<IUser>(user)
        };
        
        return result;
    }

    private async Task SetCommonChatCountAsync(IRequestInput input, IUserReadModel userReadModel, IUserFull userFull)
    {
        var count = await queryProcessor.ProcessAsync(new GetCommonChatCountQuery(input.UserId, userReadModel.UserId));
        userFull.CommonChatsCount = count;
    }

    private async Task SetPersonalChannelAsync(IRequestInput input, IUserReadModel userReadModel, IUserFull userFull)
    {
        if (userReadModel.PersonalChannelId.HasValue)
        {
            var channelTopMessageId =
                await queryProcessor.ProcessAsync(
                    new GetChannelTopMessageIdQuery(userReadModel.PersonalChannelId.Value));

            if (channelTopMessageId.HasValue)
            {
                userFull.PersonalChannelId = userReadModel.PersonalChannelId;
                userFull.PersonalChannelMessage = channelTopMessageId.Value;
            }
        }
    }

    private async Task SetBotVerificationAsync(IRequestInput input, IUserReadModel userReadModel, IUserFull userFull)
    {
            
        if (userReadModel.BotVerifierId.HasValue)
        {
            logger.LogWarning("Querying BotVerificationReadModel for TargetType=User, TargetId={TargetId}", userReadModel.UserId);
            
            var verification = await queryProcessor.ProcessAsync(
                new GetBotVerificationByTargetQuery(VerificationTargetType.User, userReadModel.UserId),
                default);

            logger.LogWarning("Query result: {HasVerification}, verification={Verification}", 
                verification != null, verification?.Description);

            if (verification != null)
            {
                logger.LogWarning("Setting BotVerification: BotId={BotId}, Icon={Icon}, Description={Description}",
                    verification.BotVerifierId, verification.IconEmojiId, verification.Description);
                    
                // Используем CustomDescription, если он не пустой, иначе берём стандартный Description
                var description = !string.IsNullOrWhiteSpace(verification.CustomDescription)
                    ? verification.CustomDescription 
                    : verification.Description;
                    
                userFull.BotVerification = new TBotVerification
                {
                    BotId = verification.BotVerifierId,
                    Icon = verification.IconEmojiId,
                    Description = description
                };
                
                logger.LogWarning("BotVerification SET: BotId={BotId}, Icon={Icon}, Description=\"{Description}\" (CustomDescription=\"{Custom}\", length={Length})",
                    verification.BotVerifierId, verification.IconEmojiId, description, verification.CustomDescription ?? "null", description?.Length ?? 0);

                // Предзагружаем документ emoji-иконки верификации
                try
                {
                    var iconDoc = await queryProcessor.ProcessAsync(
                        new GetDocumentByIdQuery(verification.IconEmojiId));
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to preload verification icon document {DocumentId}", verification.IconEmojiId);
                }
            }
            else
            {
                logger.LogWarning("Verification is NULL after query!");
            }
        }
    }

    private async Task SetSavedMusicAsync(IRequestInput input, IUserReadModel userReadModel, IUserFull userFull)
    {
        // Загружаем сохранённую музыку пользователя
        var savedMusic = await queryProcessor.ProcessAsync(
            new GetSavedMusicByUserIdQuery(userReadModel.UserId),
            CancellationToken.None);

        if (savedMusic != null && savedMusic.DocumentIds.Count > 0)
        {
            // Берём первый документ
            var firstDocumentId = savedMusic.DocumentIds[0];
            var document = await queryProcessor.ProcessAsync(
                new GetDocumentByIdQuery(firstDocumentId),
                CancellationToken.None);

            if (document != null)
            {
                userFull.SavedMusic = new TDocument
                {
                    Id = document.DocumentId,
                    AccessHash = document.AccessHash,
                    FileReference = document.FileReference.IsEmpty ? Array.Empty<byte>() : document.FileReference.ToArray(),
                    Date = document.Date,
                    MimeType = document.MimeType ?? "audio/mpeg",
                    Size = document.Size,
                    DcId = document.DcId,
                    Attributes = document.Attributes2 != null
                        ? new TVector<IDocumentAttribute>(document.Attributes2)
                        : new TVector<IDocumentAttribute>()
                };

                logger.LogInformation(
                    "Set saved_music for user {UserId}: DocumentId={DocumentId}",
                    userReadModel.UserId,
                    firstDocumentId);
            }
        }
    }


    private async Task SetBusinessIntroAsync(IRequestInput input, IUserReadModel userReadModel, IUserFull userFull)
    {
        // if (userReadModel.BusinessIntro != null)
        // {
        // logger.LogInformation("Loading business intro for user {UserId}", userReadModel.UserId);

        IDocument? stickerDocument = null;
        long stickerDocId = 0;
        string title = userReadModel.BusinessIntro?.Title ?? string.Empty;
        string description = userReadModel.BusinessIntro?.Description ?? string.Empty;
        string stickerDocumentId = userReadModel.BusinessIntro?.StickerDocumentId ?? "0";

        // Проверяем маркер "0" - признак динамического случайного стикера
        if (stickerDocumentId == "0")
        {
            var randomId = await stickerAppService.GetRandomFeaturedStickerIdAsync("👋");
            if (randomId.HasValue)
            {
                stickerDocId = randomId.Value;
            }
        }
        // Иначе загружаем конкретный документ стикера, если он указан
        else if (!string.IsNullOrEmpty(stickerDocumentId))
        {
            long.TryParse(stickerDocumentId, out stickerDocId);
        }

        if (stickerDocId > 0)
        {
            try
            {
                var docReadModel = await queryProcessor.ProcessAsync(
                    new GetDocumentByIdQuery(stickerDocId),
                    CancellationToken.None);

                if (docReadModel != null)
                {
                    stickerDocument = new TDocument
                    {
                        Id = docReadModel.DocumentId,
                        AccessHash = docReadModel.AccessHash,
                        FileReference = docReadModel.FileReference.IsEmpty ? Array.Empty<byte>() : docReadModel.FileReference.ToArray(),
                        Date = docReadModel.Date,
                        MimeType = docReadModel.MimeType ?? "application/x-tgsticker",
                        Size = docReadModel.Size,
                        DcId = docReadModel.DcId,
                        Attributes = docReadModel.Attributes2 != null
                            ? new TVector<IDocumentAttribute>(docReadModel.Attributes2)
                            : new TVector<IDocumentAttribute>()
                    };

                    // Если выбран случайный стикер, а заголовок пуст, подставляем в него эмодзи
                    if (stickerDocumentId == "0" && string.IsNullOrEmpty(title) && stickerDocument is TDocument tDoc)
                    {
                        var stickerAttribute = tDoc.Attributes.FirstOrDefault(p => p is MyTelegram.Schema.TDocumentAttributeSticker) as MyTelegram.Schema.TDocumentAttributeSticker;
                        if (stickerAttribute != null && !string.IsNullOrEmpty(stickerAttribute.Alt))
                        {
                            title = stickerAttribute.Alt;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load business intro sticker document {DocumentId} for user {UserId}",
                    stickerDocId, userReadModel.UserId);
            }
        }

        if (stickerDocument != null || !string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description))
        {
            userFull.BusinessIntro = new TBusinessIntro
            {
                Title = title,
                Description = description,
                Sticker = stickerDocument
            };
        }
        // }
    }

    private async Task<int> GetStarGiftsCountAsync(long userId)
    {
        try
        {
            var gifts = await queryProcessor.ProcessAsync(
                new MyTelegram.Queries.StarGift.GetUserStarGiftsQuery(userId, "", 100)); // Берём больше подарков, чтобы точнее посчитать

            // Возвращаем количество из ответа
            if (gifts is Schema.Payments.TUserStarGifts userStarGifts)
            {
                return userStarGifts.Count;
            }
            
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get star gifts count for user {UserId}", userId);
            return 0;
        }
    }
}
