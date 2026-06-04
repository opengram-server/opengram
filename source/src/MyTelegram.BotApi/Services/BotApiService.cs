using MyTelegram.BotApi.Helpers;
using MongoDB.Driver;
using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.ReadModel.MongoDB;
using MyTelegram.Schema;
using MyTelegram.EventBus;
using System.Text.Json;
using MongoDB.Driver;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Реализация сервиса Bot API
/// </summary>
public class BotApiService(
    IMongoDatabase database,
    MTProtoBridge mtprotoBridge,
    ILogger<BotApiService> logger,
    IUpdatesManager updatesManager,
    IEventBus eventBus) : IBotApiService
{
    #region Basic Methods

    // Проверяем токен бота перед обработкой запроса
    public async Task<bool> ValidateBotTokenAsync(string token)
    {
        try
        {
            var botsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
            var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
            return bot != null; // токен валиден, если бот с ним найден
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating bot token");
            return false;
        }
    }

    public async Task<BotApiUser> GetMeAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} calling getMe", bot.UserId);
        
        // Берём данные пользователя бота из MongoDB
        var usersCollection = database.GetCollection<UserReadModel>("ReadModel-UserReadModel");
        var user = await usersCollection.Find(u => u.UserId == bot.UserId).FirstOrDefaultAsync();
        
        return new BotApiUser
        {
            Id = bot.UserId,
            IsBot = true,
            FirstName = user?.FirstName ?? bot.BotName,
            Username = user?.UserName ?? bot.UserName,
            CanJoinGroups = bot.AllowJoinGroups,
            CanReadAllGroupMessages = bot.AllowAccessGroupMessages,
            SupportsInlineQueries = bot.InlineModeEnabled
        };
    }

    public async Task<List<BotApiUpdate>> GetUpdatesAsync(string token, int offset, int limit, int timeout)
    {
        // Ограничиваем таймаут двумя секундами вместо стандартных 30 ради быстрого отклика
        var actualTimeout = Math.Min(timeout, 2);
        logger.LogInformation("Getting updates: offset={Offset}, limit={Limit}, timeout={Timeout} (capped to {ActualTimeout}s)", 
            offset, limit, timeout, actualTimeout);
        
        // Достаём обновления из UpdatesManager (очередь в памяти)
        var updates = await updatesManager.GetUpdatesAsync(token, offset, Math.Min(limit, 100), actualTimeout, null);
        
        logger.LogInformation("Returning {Count} updates", updates.Count);
        
        return updates;
    }

    public async Task<bool> SetWebhookAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var url = body.GetProperty("url").GetString();
        
        // Проверяем URL вебхука
        var (isValid, error) = InputValidationService.ValidateUrl(url);
        if (!isValid)
        {
            logger.LogWarning("Invalid webhook URL from bot {BotId}: {Error}", bot.UserId, error);
            throw new Exception(error);
        }
        
        logger.LogInformation("Bot {BotId} setting webhook: {Url}", bot.UserId, url);

        // TODO: сохранить вебхук в базу
        return true;
    }

    public async Task<bool> DeleteWebhookAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} deleting webhook", bot.UserId);

        // TODO: удалить вебхук из базы
        return true;
    }

    public async Task<object> GetWebhookInfoAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} getting webhook info", bot.UserId);
        
        return new BotApiWebhookInfo
        {
            Url = "",
            HasCustomCertificate = false,
            PendingUpdateCount = 0
        };
    }

    #endregion

    #region Send Message Methods

    public async Task<BotApiMessage> SendMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var text = body.GetProperty("text").GetString() ?? "";
        
        // Проверяем длину и содержимое входных данных
        var (isValidText, textError, sanitizedText) = InputValidationService.ValidateMessageText(text);
        if (!isValidText)
        {
            logger.LogWarning("Invalid message text from bot {BotId}: {Error}", bot.UserId, textError);
            throw new Exception(textError);
        }
        
        var (isValidChatId, chatIdError) = InputValidationService.ValidateChatId(chatId);
        if (!isValidChatId)
        {
            throw new Exception(chatIdError);
        }
        
        logger.LogInformation("Bot {BotId} sending message to {ChatId}", bot.UserId, chatId);
        
        // Разбираем необязательные параметры
        var parseMode = BotApiHelpers.GetOptionalString(body, "parse_mode");
        var disableWebPagePreview = BotApiHelpers.GetOptionalBool(body, "disable_web_page_preview");
        var disableNotification = BotApiHelpers.GetOptionalBool(body, "disable_notification");
        var replyToMessageId = BotApiHelpers.GetOptionalInt(body, "reply_to_message_id");
        
        // Разбираем клавиатуру (reply markup)
        IReplyMarkup? replyMarkup = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkup = BotApiHelpers.ParseReplyMarkup(replyMarkupJson);
        }

        // Отправляем сообщение через MTProtoBridge
        logger.LogInformation("Bot {BotId} sending message via MTProtoBridge to chat {ChatId}", bot.UserId, chatId);

        // Сейчас MTProtoBridge.SendMessageAsync возвращает заглушку
        // TODO: реализовать реальную отправку сообщений через MTProto
        var result = await mtprotoBridge.SendMessageAsync(
            botUserId: bot.UserId,
            chatId: chatId,
            text: text,
            parseMode: parseMode,
            disableWebPagePreview: disableWebPagePreview,
            disableNotification: disableNotification,
            replyToMessageId: replyToMessageId,
            replyMarkup: replyMarkup
        );
        
        logger.LogInformation("Bot {BotId} message sent (stub) - MessageId={MessageId}", bot.UserId, result.MessageId);

        // Возвращаем результат из MTProtoBridge
        return result;
    }

    public async Task<BotApiMessage> ForwardMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var fromChatId = body.GetProperty("from_chat_id").GetInt64();
        var messageId = body.GetProperty("message_id").GetInt32();
        
        logger.LogInformation("Bot {BotId} forwarding message {MessageId} from {FromChatId} to {ChatId}", 
            bot.UserId, messageId, fromChatId, chatId);
        
        var disableNotification = BotApiHelpers.GetOptionalBool(body, "disable_notification");
        
        var (toPeerId, toPeerType) = BotApiConverter.FromBotApiChatId(chatId);
        var (fromPeerId, fromPeerType) = BotApiConverter.FromBotApiChatId(fromChatId);
        
        // TODO: реализовать пересылку через ICommandBus
        // Пока возвращаем заглушку
        return new BotApiMessage
        {
            MessageId = messageId,
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = BotApiConverter.ToBotApiChat(toPeerId, toPeerType),
            Text = "[Forwarded message]"
        };
    }

    public async Task<int> CopyMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var fromChatId = body.GetProperty("from_chat_id").GetInt64();
        var messageId = body.GetProperty("message_id").GetInt32();
        
        logger.LogInformation("Bot {BotId} copying message {MessageId} from {FromChatId} to {ChatId}", 
            bot.UserId, messageId, fromChatId, chatId);
        
        // TODO: реализовать копирование через ICommandBus
        return Random.Shared.Next(1, 1000000);
    }

    #endregion

    #region Edit/Delete Message Methods

    public async Task<BotApiMessage> EditMessageTextAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        
        long? chatId = null;
        int? messageId = null;
        string? inlineMessageId = null;
        
        if (body.TryGetProperty("chat_id", out var chatIdProp))
        {
            chatId = chatIdProp.GetInt64();
        }
        if (body.TryGetProperty("message_id", out var messageIdProp))
        {
            messageId = messageIdProp.GetInt32();
        }
        if (body.TryGetProperty("inline_message_id", out var inlineMessageIdProp))
        {
            inlineMessageId = inlineMessageIdProp.GetString();
        }
        
        var text = body.GetProperty("text").GetString() ?? "";
        var parseMode = BotApiHelpers.GetOptionalString(body, "parse_mode");
        
        logger.LogInformation("Bot {BotId} editing message {MessageId} in chat {ChatId}", 
            bot.UserId, messageId, chatId);
        
        // Разбираем сущности (entities) текста
        List<IMessageEntity>? entities = null;
        if (body.TryGetProperty("entities", out var entitiesJson))
        {
            entities = ParseBotApiEntities(entitiesJson);
        }
        entities = BotApiHelpers.ParseEntities(text, parseMode, entities);

        // Разбираем клавиатуру (reply markup)
        IReplyMarkup? replyMarkup = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkup = BotApiHelpers.ParseReplyMarkup(replyMarkupJson);
        }

        // TODO: реализовать редактирование через ICommandBus

        if (chatId.HasValue && messageId.HasValue)
        {
            var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId.Value);
            
            return new BotApiMessage
            {
                MessageId = messageId.Value,
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
                Text = text,
                EditDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Entities = entities?.Select(BotApiConverter.ToBotApiMessageEntity).ToList()
            };
        }
        
        throw new Exception("Either chat_id and message_id or inline_message_id must be specified");
    }

    public async Task<BotApiMessage> EditMessageReplyMarkupAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        
        long? chatId = null;
        int? messageId = null;
        
        if (body.TryGetProperty("chat_id", out var chatIdProp))
        {
            chatId = chatIdProp.GetInt64();
        }
        if (body.TryGetProperty("message_id", out var messageIdProp))
        {
            messageId = messageIdProp.GetInt32();
        }
        
        logger.LogInformation("Bot {BotId} editing reply markup for message {MessageId} in chat {ChatId}", 
            bot.UserId, messageId, chatId);
        
        // Разбираем клавиатуру (reply markup)
        IReplyMarkup? replyMarkup = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkup = BotApiHelpers.ParseReplyMarkup(replyMarkupJson);
        }

        // TODO: реализовать через ICommandBus

        if (chatId.HasValue && messageId.HasValue)
        {
            var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId.Value);
            
            return new BotApiMessage
            {
                MessageId = messageId.Value,
                Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
                EditDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
            };
        }
        
        throw new Exception("chat_id and message_id must be specified");
    }

    public async Task<bool> DeleteMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var messageId = body.GetProperty("message_id").GetInt32();
        
        logger.LogInformation("Bot {BotId} deleting message {MessageId} from chat {ChatId}", 
            bot.UserId, messageId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);

        // TODO: реализовать удаление через ICommandBus (StartDeleteMessagesCommand)

        return true;
    }

    #endregion

    #region Callback Query

    public async Task<bool> AnswerCallbackQueryAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var callbackQueryId = body.GetProperty("callback_query_id").GetString() ?? "";
        
        var text = BotApiHelpers.GetOptionalString(body, "text");
        var showAlert = BotApiHelpers.GetOptionalBool(body, "show_alert");
        var url = BotApiHelpers.GetOptionalString(body, "url");
        var cacheTime = BotApiHelpers.GetOptionalInt(body, "cache_time") ?? 0;
        
        logger.LogInformation("Bot {BotId} answering callback query {QueryId}", bot.UserId, callbackQueryId);
        
        // Разбираем идентификатор запроса
        if (!long.TryParse(callbackQueryId, out var queryId))
        {
            throw new ArgumentException("Invalid callback_query_id format", nameof(callbackQueryId));
        }

        // TODO: реализовать ответ на callback-запрос
        logger.LogWarning("Bot {BotId} answerCallbackQuery not implemented - use Python bot API instead", bot.UserId);
        
        return true;
    }

    #endregion

    #region Chat Action

    public async Task SendChatActionAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var action = body.GetProperty("action").GetString() ?? "typing";
        
        logger.LogInformation("Bot {BotId} sending chat action {Action} to {ChatId}", bot.UserId, action, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        // Сопоставляем действие Bot API с MTProto SendMessageAction
        ISendMessageAction sendMessageAction = action switch
        {
            "typing" => new TSendMessageTypingAction(),
            "upload_photo" => new TSendMessageUploadPhotoAction { Progress = 0 },
            "record_video" => new TSendMessageRecordVideoAction(),
            "upload_video" => new TSendMessageUploadVideoAction { Progress = 0 },
            "record_voice" => new TSendMessageRecordAudioAction(),
            "upload_voice" => new TSendMessageUploadAudioAction { Progress = 0 },
            "upload_document" => new TSendMessageUploadDocumentAction { Progress = 0 },
            "choose_sticker" => new TSendMessageChooseStickerAction(),
            "find_location" => new TSendMessageGeoLocationAction(),
            "record_video_note" => new TSendMessageRecordRoundAction(),
            "upload_video_note" => new TSendMessageUploadRoundAction { Progress = 0 },
            _ => new TSendMessageTypingAction()
        };
        
        // TODO: отправить действие через MTProto (метод messages.setTyping)
    }

    #endregion

    #region Media Methods

    public Task<BotApiMessage> SendPhotoAsync(string token, long chatId, string? photo, IFormFile? photoFile)
    {
        logger.LogInformation("sendPhoto called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendPhoto - requires file upload implementation");
    }

    public Task<BotApiMessage> SendAudioAsync(string token, long chatId, string? audio, IFormFile? audioFile)
    {
        logger.LogInformation("sendAudio called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendAudio - requires file upload implementation");
    }

    public Task<BotApiMessage> SendDocumentAsync(string token, long chatId, string? document, IFormFile? documentFile)
    {
        logger.LogInformation("sendDocument called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendDocument - requires file upload implementation");
    }

    public Task<BotApiMessage> SendVideoAsync(string token, long chatId, string? video, IFormFile? videoFile)
    {
        logger.LogInformation("sendVideo called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendVideo - requires file upload implementation");
    }

    public Task<BotApiMessage> SendAnimationAsync(string token, long chatId, string? animation, IFormFile? animationFile)
    {
        logger.LogInformation("sendAnimation called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendAnimation - requires file upload implementation");
    }

    public Task<BotApiMessage> SendVoiceAsync(string token, long chatId, string? voice, IFormFile? voiceFile)
    {
        logger.LogInformation("sendVoice called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendVoice - requires file upload implementation");
    }

    public Task<BotApiMessage> SendVideoNoteAsync(string token, long chatId, string? videoNote, IFormFile? videoNoteFile)
    {
        logger.LogInformation("sendVideoNote called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendVideoNote - requires file upload implementation");
    }

    public Task<BotApiMessage> SendStickerAsync(string token, long chatId, string? sticker, IFormFile? stickerFile)
    {
        logger.LogInformation("sendSticker called for chat {ChatId}", chatId);
        throw new NotImplementedException("sendSticker - requires file upload implementation");
    }

    public Task<List<BotApiMessage>> SendMediaGroupAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendMediaGroup - requires media group implementation");
    }

    public Task<BotApiMessage> SendLocationAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendLocation");
    }

    public Task<BotApiMessage> SendVenueAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendVenue");
    }

    public Task<BotApiMessage> SendContactAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendContact");
    }

    public Task<BotApiMessage> SendPollAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendPoll");
    }

    public Task<BotApiMessage> SendDiceAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("sendDice");
    }

    #endregion

    #region Other Methods

    public Task<object> GetUserProfilePhotosAsync(string token, long userId, int? offset, int? limit)
    {
        throw new NotImplementedException("getUserProfilePhotos");
    }

    public Task<object> GetFileAsync(string token, string fileId)
    {
        throw new NotImplementedException("getFile");
    }

    public Task BanChatMemberAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("banChatMember");
    }

    public Task UnbanChatMemberAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("unbanChatMember");
    }

    public Task RestrictChatMemberAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("restrictChatMember");
    }

    public Task PromoteChatMemberAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("promoteChatMember");
    }

    public Task SetChatAdministratorCustomTitleAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("setChatAdministratorCustomTitle");
    }

    public Task BanChatSenderChatAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("banChatSenderChat");
    }

    public Task UnbanChatSenderChatAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("unbanChatSenderChat");
    }

    public Task SetChatPermissionsAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("setChatPermissions");
    }

    public Task<string> ExportChatInviteLinkAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("exportChatInviteLink");
    }

    public Task<object> CreateChatInviteLinkAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("createChatInviteLink");
    }

    public Task<object> EditChatInviteLinkAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("editChatInviteLink");
    }

    public Task<object> RevokeChatInviteLinkAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("revokeChatInviteLink");
    }

    public Task ApproveChatJoinRequestAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("approveChatJoinRequest");
    }

    public Task DeclineChatJoinRequestAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("declineChatJoinRequest");
    }

    public Task SetChatPhotoAsync(string token, long chatId, IFormFile photo)
    {
        throw new NotImplementedException("setChatPhoto");
    }

    public Task DeleteChatPhotoAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("deleteChatPhoto");
    }

    public Task SetChatTitleAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("setChatTitle");
    }

    public Task SetChatDescriptionAsync(string token, JsonElement body)
    {
        throw new NotImplementedException("setChatDescription");
    }

    #endregion

    #region Helper Methods

    private async Task<MyTelegram.ReadModel.Impl.BotReadModel> GetBotByTokenAsync(string token)
    {
        // Query bot directly from MongoDB
        var botsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.BotReadModel>("ReadModel-BotReadModel");
        var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
        
        if (bot == null)
        {
            throw new Exception("Invalid bot token");
        }
        return bot;
    }

    private List<IMessageEntity>? ParseBotApiEntities(JsonElement entitiesJson)
    {
        if (entitiesJson.ValueKind != JsonValueKind.Array)
        {
            return null;
        }

        var entities = new List<IMessageEntity>();
        
        foreach (var entity in entitiesJson.EnumerateArray())
        {
            var type = entity.GetProperty("type").GetString();
            var offset = entity.GetProperty("offset").GetInt32();
            var length = entity.GetProperty("length").GetInt32();
            
            IMessageEntity messageEntity = type switch
            {
                "bold" => new TMessageEntityBold { Offset = offset, Length = length },
                "italic" => new TMessageEntityItalic { Offset = offset, Length = length },
                "underline" => new TMessageEntityUnderline { Offset = offset, Length = length },
                "strikethrough" => new TMessageEntityStrike { Offset = offset, Length = length },
                "spoiler" => new TMessageEntitySpoiler { Offset = offset, Length = length },
                "code" => new TMessageEntityCode { Offset = offset, Length = length },
                "pre" => new TMessageEntityPre 
                { 
                    Offset = offset, 
                    Length = length,
                    Language = entity.TryGetProperty("language", out var lang) ? lang.GetString() ?? "" : ""
                },
                "text_link" => new TMessageEntityTextUrl 
                { 
                    Offset = offset, 
                    Length = length,
                    Url = entity.GetProperty("url").GetString() ?? ""
                },
                "text_mention" => new TMessageEntityMentionName
                {
                    Offset = offset,
                    Length = length,
                    UserId = entity.GetProperty("user").GetProperty("id").GetInt64()
                },
                "custom_emoji" => new TMessageEntityCustomEmoji
                {
                    Offset = offset,
                    Length = length,
                    DocumentId = long.Parse(entity.GetProperty("custom_emoji_id").GetString() ?? "0")
                },
                _ => new TMessageEntityUnknown { Offset = offset, Length = length }
            };
            
            entities.Add(messageEntity);
        }
        
        return entities.Count > 0 ? entities : null;
    }

    #endregion

    #region Star Gifts

    public async Task<object> GetAvailableGiftsAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} calling getAvailableGifts", bot.UserId);
        
        // Загружаем подарки напрямую из MongoDB
        var giftsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var giftsReadModels = await giftsCollection.Find(_ => true).ToListAsync();
        
        var gifts = new List<object>();
        
        foreach (var gift in giftsReadModels)
        {
            // Подгружаем документ стикера для подарка
            object? sticker = null;
            if (gift.Sticker > 0)
            {
                try
                {
                    var documentsCollection = database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
                    var document = await documentsCollection.Find(d => d.DocumentId == gift.Sticker).FirstOrDefaultAsync();
                    
                    if (document != null)
                    {
                        sticker = new
                        {
                            file_id = $"doc_{document.DocumentId}",
                            file_unique_id = $"doc_{document.DocumentId}_unique",
                            width = 512,
                            height = 512,
                            is_animated = document.MimeType == "application/x-tgsticker",
                            is_video = false,
                            type = "regular",
                            thumbnail = (object?)null,
                            emoji = gift.FirstSaleDate > 0 ? "⭐" : "🎁",
                            set_name = "StarGifts",
                            file_size = document.Size
                        };
                    }
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to load sticker for gift {GiftId}", gift.GiftId);
                }
            }
            
            gifts.Add(new
            {
                id = gift.GiftId.ToString(),
                sticker,
                star_count = gift.Stars,
                total_count = gift.AvailabilityTotal,
                remaining_count = gift.AvailabilityRemains
            });
        }
        
        return new { gifts };
    }

    public async Task<bool> SendGiftAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var userId = body.GetProperty("user_id").GetInt64();
        var giftIdStr = body.GetProperty("gift_id").GetString();
        var giftId = long.Parse(giftIdStr!);
        var text = BotApiHelpers.GetOptionalString(body, "text");
        var parseMode = BotApiHelpers.GetOptionalString(body, "text_parse_mode");
        _ = parseMode;
        var hideName = body.TryGetProperty("hide_name", out var hideNameElement) && hideNameElement.GetBoolean();
        var includeUpgrade = body.TryGetProperty("include_upgrade", out var includeUpgradeElement) && includeUpgradeElement.GetBoolean();
        var count = body.TryGetProperty("count", out var countElement) ? Math.Max(1, countElement.GetInt32()) : 1;
        
        logger.LogInformation("Bot {BotId} sending gift {GiftId} to user {UserId} (Count={Count}, HideName={HideName}, IncludeUpgrade={IncludeUpgrade})", 
            bot.UserId, giftId, userId, count, hideName, includeUpgrade);
        
        // Загружаем сведения о подарке из базы
        var giftsCollection = database.GetCollection<MyTelegram.ReadModel.Impl.AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var gift = await giftsCollection.Find(g => g.GiftId == giftId).FirstOrDefaultAsync();
        
        if (gift == null)
        {
            logger.LogWarning("Gift {GiftId} not found", giftId);
            return false;
        }
        
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var randomId = Random.Shared.NextInt64();
        
        var giftEvent = new BotGiftEvent
        {
            BotUserId = bot.UserId,
            ToUserId = userId,
            GiftId = giftId,
            Count = count,
            Message = text,
            HideName = hideName,
            IncludeUpgrade = includeUpgrade,
            Timestamp = now,
            RandomId = randomId
        };
        
        await eventBus.PublishAsync(giftEvent);
        
        logger.LogInformation("Published BotGiftEvent for bot {BotId} to user {UserId} (GiftId={GiftId}, Count={Count})",
            bot.UserId, userId, giftId, count);
        
        return true;
    }

    #endregion

    #region Stars Payment API

    public async Task<BotApiMessage> SendInvoiceAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var title = body.GetProperty("title").GetString() ?? "";
        var description = body.GetProperty("description").GetString() ?? "";
        var payload = body.GetProperty("payload").GetString() ?? "";
        var currency = body.GetProperty("currency").GetString() ?? "XTR"; // валюта Stars

        // Разбираем массив цен
        var pricesJson = body.GetProperty("prices");
        var prices = new List<BotApiLabeledPrice>();
        int totalAmount = 0;
        
        foreach (var priceElement in pricesJson.EnumerateArray())
        {
            var label = priceElement.GetProperty("label").GetString() ?? "";
            var amount = priceElement.GetProperty("amount").GetInt32();
            prices.Add(new BotApiLabeledPrice { Label = label, Amount = amount });
            totalAmount += amount;
        }
        
        logger.LogInformation("Bot {BotId} sending invoice to {ChatId}: {Title} - {Amount} {Currency}", 
            bot.UserId, chatId, title, totalAmount, currency);
        
        // Формируем счёт (invoice)
        var invoice = new BotApiInvoice
        {
            Title = title,
            Description = description,
            StartParameter = payload,
            Currency = currency,
            TotalAmount = totalAmount
        };
        
        // Создаём сообщение со счётом
        var message = new BotApiMessage
        {
            MessageId = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = new BotApiChat
            {
                Id = chatId,
                Type = "private"
            },
            From = new BotApiUser
            {
                Id = bot.UserId,
                IsBot = true,
                FirstName = bot.BotName
            },
            Invoice = invoice
        };
        
        // Сохраняем счёт в памяти для проверки на этапе pre-checkout
        await updatesManager.StoreInvoiceAsync(payload, new
        {
            bot_id = bot.UserId,
            chat_id = chatId,
            title,
            description,
            payload,
            currency,
            total_amount = totalAmount,
            prices
        });
        
        logger.LogInformation("Invoice created with payload: {Payload}", payload);
        
        return message;
    }

    public async Task<bool> AnswerPreCheckoutQueryAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var preCheckoutQueryId = body.GetProperty("pre_checkout_query_id").GetString() ?? "";
        var ok = body.TryGetProperty("ok", out var okElement) && okElement.GetBoolean();
        var errorMessage = BotApiHelpers.GetOptionalString(body, "error_message");
        
        logger.LogInformation("Bot {BotId} answering pre-checkout query {QueryId}: ok={Ok}", 
            bot.UserId, preCheckoutQueryId, ok);
        
        if (!ok && !string.IsNullOrEmpty(errorMessage))
        {
            logger.LogWarning("Pre-checkout declined: {Error}", errorMessage);
        }
        
        // Передаём ответ пользователю через UpdatesManager
        await updatesManager.AnswerPreCheckoutAsync(preCheckoutQueryId, ok, errorMessage);
        
        return true;
    }

    #endregion
}
