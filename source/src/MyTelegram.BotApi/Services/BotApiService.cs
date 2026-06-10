using MyTelegram.BotApi.Helpers;
using MongoDB.Driver;
using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.ReadModel.Impl;
using MyTelegram.Schema;
using MyTelegram.EventBus;
using System.Text.Json;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Production implementation of Bot API service.
/// Reads from MongoDB, writes via RabbitMQ events through MTProtoBridge.
/// </summary>
public class BotApiService(
    IMongoDatabase database,
    MTProtoBridge mtprotoBridge,
    ILogger<BotApiService> logger,
    IUpdatesManager updatesManager,
    IEventBus eventBus) : IBotApiService
{
    #region Basic Methods

    public async Task<bool> ValidateBotTokenAsync(string token)
    {
        try
        {
            var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
            var bot = await botsCollection.Find(b => b.Token == token).FirstOrDefaultAsync();
            return bot != null;
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
        var actualTimeout = Math.Min(timeout, 30);
        logger.LogDebug("Getting updates: offset={Offset}, limit={Limit}, timeout={Timeout}",
            offset, limit, actualTimeout);

        var updates = await updatesManager.GetUpdatesAsync(token, offset, Math.Min(limit, 100), actualTimeout, null);

        logger.LogDebug("Returning {Count} updates", updates.Count);
        return updates;
    }

    public async Task<bool> SetWebhookAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var url = body.GetProperty("url").GetString();

        var (isValid, error) = InputValidationService.ValidateUrl(url);
        if (!isValid)
        {
            logger.LogWarning("Invalid webhook URL from bot {BotId}: {Error}", bot.UserId, error);
            throw new Exception(error);
        }

        var secretToken = BotApiHelpers.GetOptionalString(body, "secret_token");
        var maxConnections = BotApiHelpers.GetOptionalInt(body, "max_connections") ?? 40;
        List<string>? allowedUpdates = null;
        if (body.TryGetProperty("allowed_updates", out var allowedUpdatesJson) &&
            allowedUpdatesJson.ValueKind == JsonValueKind.Array)
        {
            allowedUpdates = allowedUpdatesJson.EnumerateArray()
                .Select(x => x.GetString() ?? "")
                .Where(x => !string.IsNullOrEmpty(x))
                .ToList();
        }

        logger.LogInformation("Bot {BotId} setting webhook: {Url} (max_connections={MaxConn})",
            bot.UserId, url, maxConnections);

        // Persist webhook via WebhookManager + update BotReadModel
        await updatesManager.SetWebhookAsync(token, url!, secretToken, maxConnections, allowedUpdates);

        return true;
    }

    public async Task<bool> DeleteWebhookAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} deleting webhook", bot.UserId);

        await updatesManager.DeleteWebhookAsync(token);
        return true;
    }

    public async Task<object> GetWebhookInfoAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} getting webhook info", bot.UserId);

        var webhookInfo = await updatesManager.GetWebhookInfoAsync(token);
        if (webhookInfo != null)
        {
            return webhookInfo;
        }

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

        var parseMode = BotApiHelpers.GetOptionalString(body, "parse_mode");
        var disableWebPagePreview = BotApiHelpers.GetOptionalBool(body, "disable_web_page_preview");
        var disableNotification = BotApiHelpers.GetOptionalBool(body, "disable_notification");
        var replyToMessageId = BotApiHelpers.GetOptionalInt(body, "reply_to_message_id");

        IReplyMarkup? replyMarkup = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkup = BotApiHelpers.ParseReplyMarkup(replyMarkupJson);
        }

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

        logger.LogInformation("Bot {BotId} message sent - MessageId={MessageId}", bot.UserId, result.MessageId);
        return result;
    }

    public async Task<BotApiMessage> ForwardMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var fromChatId = body.GetProperty("from_chat_id").GetInt64();
        var messageId = body.GetProperty("message_id").GetInt32();
        var disableNotification = BotApiHelpers.GetOptionalBool(body, "disable_notification");
        var protectContent = BotApiHelpers.GetOptionalBool(body, "protect_content");

        logger.LogInformation("Bot {BotId} forwarding message {MessageId} from {FromChatId} to {ChatId}",
            bot.UserId, messageId, fromChatId, chatId);

        return await mtprotoBridge.ForwardMessageAsync(
            bot.UserId, chatId, fromChatId, messageId, disableNotification, protectContent);
    }

    public async Task<int> CopyMessageAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var fromChatId = body.GetProperty("from_chat_id").GetInt64();
        var messageId = body.GetProperty("message_id").GetInt32();
        var caption = BotApiHelpers.GetOptionalString(body, "caption");
        var parseMode = BotApiHelpers.GetOptionalString(body, "parse_mode");

        logger.LogInformation("Bot {BotId} copying message {MessageId} from {FromChatId} to {ChatId}",
            bot.UserId, messageId, fromChatId, chatId);

        // Read original message from MongoDB
        var (fromPeerId, _) = BotApiConverter.FromBotApiChatId(fromChatId);
        var messagesCollection = database.GetCollection<MessageReadModel>("ReadModel-MessageReadModel");
        var originalMsg = await messagesCollection
            .Find(m => m.OwnerPeerId == fromPeerId && m.MessageId == messageId)
            .FirstOrDefaultAsync();

        var textToSend = caption ?? originalMsg?.Message ?? "";

        // Send as a new message
        var result = await mtprotoBridge.SendMessageAsync(
            bot.UserId, chatId, textToSend, parseMode);

        return result.MessageId;
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

        // Parse entities
        List<IMessageEntity>? entities = null;
        if (body.TryGetProperty("entities", out var entitiesJson))
        {
            entities = ParseBotApiEntities(entitiesJson);
        }
        entities = BotApiHelpers.ParseEntities(text, parseMode, entities);

        // Parse reply_markup
        string? replyMarkupJsonStr = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkupJsonStr = replyMarkupJson.GetRawText();
        }

        if (chatId.HasValue && messageId.HasValue)
        {
            var result = await mtprotoBridge.EditMessageTextAsync(
                bot.UserId, chatId.Value, messageId.Value, text, parseMode, replyMarkupJsonStr);

            result.Entities = entities?.Select(BotApiConverter.ToBotApiMessageEntity).ToList();
            return result;
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

        string? replyMarkupJsonStr = null;
        if (body.TryGetProperty("reply_markup", out var replyMarkupJson))
        {
            replyMarkupJsonStr = replyMarkupJson.GetRawText();
        }

        if (chatId.HasValue && messageId.HasValue)
        {
            return await mtprotoBridge.EditMessageReplyMarkupAsync(
                bot.UserId, chatId.Value, messageId.Value, replyMarkupJsonStr);
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

        return await mtprotoBridge.DeleteMessageAsync(bot.UserId, chatId, messageId);
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

        if (!long.TryParse(callbackQueryId, out var queryId))
        {
            throw new ArgumentException("Invalid callback_query_id format", nameof(callbackQueryId));
        }

        return await mtprotoBridge.AnswerCallbackQueryAsync(
            bot.UserId, queryId, text, showAlert, url, cacheTime);
    }

    #endregion

    #region Chat Action

    public async Task SendChatActionAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var action = body.GetProperty("action").GetString() ?? "typing";
        var messageThreadId = BotApiHelpers.GetOptionalInt(body, "message_thread_id");

        logger.LogDebug("Bot {BotId} sending chat action {Action} to {ChatId}", bot.UserId, action, chatId);

        await mtprotoBridge.SendChatActionAsync(bot.UserId, chatId, action, messageThreadId);
    }

    #endregion

    #region Media Methods

    public async Task<BotApiMessage> SendPhotoAsync(string token, long chatId, string? photo, IFormFile? photoFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending photo to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Photo",
            FileId = photo
        };

        if (photoFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(photoFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendAudioAsync(string token, long chatId, string? audio, IFormFile? audioFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending audio to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Audio",
            FileId = audio
        };

        if (audioFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(audioFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendDocumentAsync(string token, long chatId, string? document, IFormFile? documentFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending document to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Document",
            FileId = document
        };

        if (documentFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(documentFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendVideoAsync(string token, long chatId, string? video, IFormFile? videoFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending video to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Video",
            FileId = video
        };

        if (videoFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(videoFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendAnimationAsync(string token, long chatId, string? animation, IFormFile? animationFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending animation to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Animation",
            FileId = animation
        };

        if (animationFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(animationFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendVoiceAsync(string token, long chatId, string? voice, IFormFile? voiceFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending voice to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Voice",
            FileId = voice
        };

        if (voiceFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(voiceFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendVideoNoteAsync(string token, long chatId, string? videoNote, IFormFile? videoNoteFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending video note to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "VideoNote",
            FileId = videoNote
        };

        if (videoNoteFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(videoNoteFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendStickerAsync(string token, long chatId, string? sticker, IFormFile? stickerFile)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} sending sticker to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Sticker",
            FileId = sticker
        };

        if (stickerFile != null)
        {
            mediaEvent.FileBase64 = await ConvertFormFileToBase64(stickerFile);
        }

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<List<BotApiMessage>> SendMediaGroupAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var media = body.GetProperty("media");

        logger.LogInformation("Bot {BotId} sending media group ({Count} items) to {ChatId}",
            bot.UserId, media.GetArrayLength(), chatId);

        var results = new List<BotApiMessage>();

        foreach (var item in media.EnumerateArray())
        {
            var type = item.GetProperty("type").GetString() ?? "photo";
            var mediaValue = item.TryGetProperty("media", out var mediaProp) ? mediaProp.GetString() : null;
            var caption = item.TryGetProperty("caption", out var captionProp) ? captionProp.GetString() : null;
            var parseMode = item.TryGetProperty("parse_mode", out var pmProp) ? pmProp.GetString() : null;

            var mediaEvent = new BotSendMediaEvent
            {
                BotUserId = bot.UserId,
                ChatId = chatId,
                MediaType = char.ToUpper(type[0]) + type[1..],
                FileId = mediaValue,
                Caption = caption,
                ParseMode = parseMode
            };

            results.Add(await mtprotoBridge.SendMediaAsync(mediaEvent));
        }

        return results;
    }

    public async Task<BotApiMessage> SendLocationAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var latitude = body.GetProperty("latitude").GetDouble();
        var longitude = body.GetProperty("longitude").GetDouble();

        logger.LogInformation("Bot {BotId} sending location ({Lat}, {Lon}) to {ChatId}",
            bot.UserId, latitude, longitude, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Location",
            Latitude = latitude,
            Longitude = longitude
        };

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendVenueAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var latitude = body.GetProperty("latitude").GetDouble();
        var longitude = body.GetProperty("longitude").GetDouble();
        var title = body.GetProperty("title").GetString() ?? "";
        var address = body.GetProperty("address").GetString() ?? "";

        logger.LogInformation("Bot {BotId} sending venue to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Venue",
            Latitude = latitude,
            Longitude = longitude,
            VenueTitle = title,
            VenueAddress = address
        };

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendContactAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var phoneNumber = body.GetProperty("phone_number").GetString() ?? "";
        var firstName = body.GetProperty("first_name").GetString() ?? "";
        var lastName = BotApiHelpers.GetOptionalString(body, "last_name");

        logger.LogInformation("Bot {BotId} sending contact to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Contact",
            PhoneNumber = phoneNumber,
            ContactFirstName = firstName,
            ContactLastName = lastName
        };

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendPollAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var question = body.GetProperty("question").GetString() ?? "";
        var options = body.GetProperty("options");
        var isAnonymous = BotApiHelpers.GetOptionalBool(body, "is_anonymous", true);
        var pollType = BotApiHelpers.GetOptionalString(body, "type") ?? "regular";

        logger.LogInformation("Bot {BotId} sending poll to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Poll",
            Question = question,
            OptionsJson = options.GetRawText(),
            IsAnonymous = isAnonymous,
            PollType = pollType
        };

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    public async Task<BotApiMessage> SendDiceAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var emoji = BotApiHelpers.GetOptionalString(body, "emoji") ?? "🎲";

        logger.LogInformation("Bot {BotId} sending dice to {ChatId}", bot.UserId, chatId);

        var mediaEvent = new BotSendMediaEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            MediaType = "Dice",
            Emoji = emoji
        };

        return await mtprotoBridge.SendMediaAsync(mediaEvent);
    }

    #endregion

    #region User & File Methods

    public async Task<object> GetUserProfilePhotosAsync(string token, long userId, int? offset, int? limit)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} getting profile photos for user {UserId}", bot.UserId, userId);

        var actualOffset = offset ?? 0;
        var actualLimit = Math.Min(limit ?? 100, 100);

        var photosCollection = database.GetCollection<PhotoReadModel>("ReadModel-PhotoReadModel");
        var photos = await photosCollection
            .Find(p => p.UserId == userId && p.IsProfilePhoto)
            .Skip(actualOffset)
            .Limit(actualLimit)
            .ToListAsync();

        var totalCount = (int)await photosCollection.CountDocumentsAsync(
            p => p.UserId == userId && p.IsProfilePhoto);

        var photoSizes = new List<List<BotApiPhotoSize>>();
        foreach (var photo in photos)
        {
            var sizes = new List<BotApiPhotoSize>();
            if (photo.Sizes != null)
            {
                foreach (var size in photo.Sizes)
                {
                    sizes.Add(new BotApiPhotoSize
                    {
                        FileId = $"photo_{photo.PhotoId}_{size.Type}",
                        FileUniqueId = $"photo_{photo.PhotoId}_{size.Type}_unique",
                        Width = size.W,
                        Height = size.H,
                        FileSize = size.Size
                    });
                }
            }
            else
            {
                // Fallback when no explicit sizes are recorded
                sizes.Add(new BotApiPhotoSize
                {
                    FileId = $"photo_{photo.PhotoId}",
                    FileUniqueId = $"photo_{photo.PhotoId}_unique",
                    Width = 320,
                    Height = 320,
                    FileSize = photo.Size
                });
            }

            photoSizes.Add(sizes);
        }

        return new BotApiUserProfilePhotos
        {
            TotalCount = totalCount,
            Photos = photoSizes
        };
    }

    public async Task<object> GetFileAsync(string token, string fileId)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} getting file {FileId}", bot.UserId, fileId);

        // Try to resolve file_id to a document
        if (fileId.StartsWith("doc_"))
        {
            var docIdStr = fileId.Replace("doc_", "").Split('_')[0];
            if (long.TryParse(docIdStr, out var documentId))
            {
                var documentsCollection = database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
                var document = await documentsCollection.Find(d => d.DocumentId == documentId).FirstOrDefaultAsync();

                if (document != null)
                {
                    return new BotApiFile
                    {
                        FileId = fileId,
                        FileUniqueId = $"doc_{documentId}_unique",
                        FileSize = document.Size,
                        FilePath = $"documents/{documentId}"
                    };
                }
            }
        }
        else if (fileId.StartsWith("photo_"))
        {
            var photoIdStr = fileId.Replace("photo_", "").Split('_')[0];
            if (long.TryParse(photoIdStr, out var photoId))
            {
                var photosCollection = database.GetCollection<PhotoReadModel>("ReadModel-PhotoReadModel");
                var photo = await photosCollection.Find(p => p.PhotoId == photoId).FirstOrDefaultAsync();

                if (photo != null)
                {
                    return new BotApiFile
                    {
                        FileId = fileId,
                        FileUniqueId = $"photo_{photoId}_unique",
                        FileSize = photo.Size,
                        FilePath = $"photos/{photoId}"
                    };
                }
            }
        }

        // Generic fallback: assume file_id is a document ID
        if (long.TryParse(fileId, out var genericId))
        {
            var documentsCollection = database.GetCollection<DocumentReadModel>("ReadModel-DocumentReadModel");
            var document = await documentsCollection.Find(d => d.DocumentId == genericId).FirstOrDefaultAsync();

            if (document != null)
            {
                return new BotApiFile
                {
                    FileId = fileId,
                    FileUniqueId = $"{genericId}_unique",
                    FileSize = document.Size,
                    FilePath = $"documents/{genericId}"
                };
            }
        }

        throw new Exception($"File not found: {fileId}");
    }

    public async Task<BotApiChat> GetChatAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();

        return await mtprotoBridge.GetChatAsync(bot.UserId, chatId);
    }

    #endregion

    #region Chat Member Management

    public async Task BanChatMemberAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();
        var untilDate = BotApiHelpers.GetOptionalInt(body, "until_date");
        var revokeMessages = BotApiHelpers.GetOptionalBool(body, "revoke_messages", true);

        logger.LogInformation("Bot {BotId} banning user {UserId} in chat {ChatId}", bot.UserId, userId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            UserId = userId,
            Action = "Ban",
            UntilDate = untilDate,
            RevokeMessages = revokeMessages,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task UnbanChatMemberAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();
        var onlyIfBanned = BotApiHelpers.GetOptionalBool(body, "only_if_banned");

        logger.LogInformation("Bot {BotId} unbanning user {UserId} in chat {ChatId}", bot.UserId, userId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            UserId = userId,
            Action = "Unban",
            OnlyIfBanned = onlyIfBanned,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task RestrictChatMemberAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();
        var permissions = body.GetProperty("permissions");
        var untilDate = BotApiHelpers.GetOptionalInt(body, "until_date");
        var useIndependent = BotApiHelpers.GetOptionalBool(body, "use_independent_chat_permissions");

        logger.LogInformation("Bot {BotId} restricting user {UserId} in chat {ChatId}", bot.UserId, userId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            UserId = userId,
            Action = "Restrict",
            PermissionsJson = permissions.GetRawText(),
            UntilDate = untilDate,
            UseIndependentChatPermissions = useIndependent,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task PromoteChatMemberAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();

        logger.LogInformation("Bot {BotId} promoting user {UserId} in chat {ChatId}", bot.UserId, userId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            UserId = userId,
            Action = "Promote",
            IsAnonymous = BotApiHelpers.GetOptionalBool(body, "is_anonymous") ? true : null,
            CanManageChat = BotApiHelpers.GetOptionalBool(body, "can_manage_chat") ? true : null,
            CanDeleteMessages = BotApiHelpers.GetOptionalBool(body, "can_delete_messages") ? true : null,
            CanManageVideoChats = BotApiHelpers.GetOptionalBool(body, "can_manage_video_chats") ? true : null,
            CanRestrictMembers = BotApiHelpers.GetOptionalBool(body, "can_restrict_members") ? true : null,
            CanPromoteMembers = BotApiHelpers.GetOptionalBool(body, "can_promote_members") ? true : null,
            CanChangeInfo = BotApiHelpers.GetOptionalBool(body, "can_change_info") ? true : null,
            CanInviteUsers = BotApiHelpers.GetOptionalBool(body, "can_invite_users") ? true : null,
            CanPostStories = BotApiHelpers.GetOptionalBool(body, "can_post_stories") ? true : null,
            CanEditStories = BotApiHelpers.GetOptionalBool(body, "can_edit_stories") ? true : null,
            CanDeleteStories = BotApiHelpers.GetOptionalBool(body, "can_delete_stories") ? true : null,
            CanPostMessages = BotApiHelpers.GetOptionalBool(body, "can_post_messages") ? true : null,
            CanEditMessages = BotApiHelpers.GetOptionalBool(body, "can_edit_messages") ? true : null,
            CanPinMessages = BotApiHelpers.GetOptionalBool(body, "can_pin_messages") ? true : null,
            CanManageTopics = BotApiHelpers.GetOptionalBool(body, "can_manage_topics") ? true : null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task SetChatAdministratorCustomTitleAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();
        var customTitle = body.GetProperty("custom_title").GetString() ?? "";

        logger.LogInformation("Bot {BotId} setting admin title for user {UserId} in chat {ChatId}: {Title}",
            bot.UserId, userId, chatId, customTitle);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            UserId = userId,
            Action = "SetCustomTitle",
            CustomTitle = customTitle,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task BanChatSenderChatAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var senderChatId = body.GetProperty("sender_chat_id").GetInt64();

        logger.LogInformation("Bot {BotId} banning sender chat {SenderChatId} in {ChatId}",
            bot.UserId, senderChatId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "BanSenderChat",
            SenderChatId = senderChatId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task UnbanChatSenderChatAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var senderChatId = body.GetProperty("sender_chat_id").GetInt64();

        logger.LogInformation("Bot {BotId} unbanning sender chat {SenderChatId} in {ChatId}",
            bot.UserId, senderChatId, chatId);

        await mtprotoBridge.ManageChatMemberAsync(new BotChatMemberEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "UnbanSenderChat",
            SenderChatId = senderChatId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }

    public async Task SetChatPermissionsAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var permissions = body.GetProperty("permissions");
        var useIndependent = BotApiHelpers.GetOptionalBool(body, "use_independent_chat_permissions");

        logger.LogInformation("Bot {BotId} setting permissions for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.SetChatPermissionsAsync(
            bot.UserId, chatId, permissions.GetRawText(), useIndependent);
    }

    #endregion

    #region Invite Links

    public async Task<string> ExportChatInviteLinkAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();

        logger.LogInformation("Bot {BotId} exporting invite link for chat {ChatId}", bot.UserId, chatId);

        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);

        // Query existing permanent invite link from MongoDB
        var invitesCollection = database.GetCollection<ChatInviteReadModel>("ReadModel-ChatInviteReadModel");
        var invite = await invitesCollection
            .Find(i => i.PeerId == peerId && i.Permanent && !i.Revoked)
            .FirstOrDefaultAsync();

        if (invite != null)
        {
            return invite.Link;
        }

        // Request creation of a new export link via event
        await mtprotoBridge.ManageChatInviteLinkAsync(new BotChatInviteLinkEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "Export",
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        // Return a placeholder — the real link will be created asynchronously
        return $"https://t.me/+pending_{peerId}";
    }

    public async Task<object> CreateChatInviteLinkAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var name = BotApiHelpers.GetOptionalString(body, "name");
        var expireDate = BotApiHelpers.GetOptionalInt(body, "expire_date");
        var memberLimit = BotApiHelpers.GetOptionalInt(body, "member_limit");
        var createsJoinRequest = BotApiHelpers.GetOptionalBool(body, "creates_join_request");

        logger.LogInformation("Bot {BotId} creating invite link for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.ManageChatInviteLinkAsync(new BotChatInviteLinkEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "Create",
            Name = name,
            ExpireDate = expireDate,
            MemberLimit = memberLimit,
            CreatesJoinRequest = createsJoinRequest,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        var (peerId, _) = BotApiConverter.FromBotApiChatId(chatId);

        return new BotApiChatInviteLink
        {
            InviteLink = $"https://t.me/+inv_{peerId}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}",
            Creator = new BotApiUser { Id = bot.UserId, IsBot = true, FirstName = bot.BotName },
            CreatesJoinRequest = createsJoinRequest,
            IsPrimary = false,
            IsRevoked = false,
            Name = name,
            ExpireDate = expireDate,
            MemberLimit = memberLimit
        };
    }

    public async Task<object> EditChatInviteLinkAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var inviteLink = body.GetProperty("invite_link").GetString() ?? "";
        var name = BotApiHelpers.GetOptionalString(body, "name");
        var expireDate = BotApiHelpers.GetOptionalInt(body, "expire_date");
        var memberLimit = BotApiHelpers.GetOptionalInt(body, "member_limit");
        var createsJoinRequest = BotApiHelpers.GetOptionalBool(body, "creates_join_request");

        logger.LogInformation("Bot {BotId} editing invite link for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.ManageChatInviteLinkAsync(new BotChatInviteLinkEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "Edit",
            InviteLink = inviteLink,
            Name = name,
            ExpireDate = expireDate,
            MemberLimit = memberLimit,
            CreatesJoinRequest = createsJoinRequest,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        return new BotApiChatInviteLink
        {
            InviteLink = inviteLink,
            Creator = new BotApiUser { Id = bot.UserId, IsBot = true, FirstName = bot.BotName },
            CreatesJoinRequest = createsJoinRequest,
            IsPrimary = false,
            IsRevoked = false,
            Name = name,
            ExpireDate = expireDate,
            MemberLimit = memberLimit
        };
    }

    public async Task<object> RevokeChatInviteLinkAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var inviteLink = body.GetProperty("invite_link").GetString() ?? "";

        logger.LogInformation("Bot {BotId} revoking invite link for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.ManageChatInviteLinkAsync(new BotChatInviteLinkEvent
        {
            BotUserId = bot.UserId,
            ChatId = chatId,
            Action = "Revoke",
            InviteLink = inviteLink,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });

        return new BotApiChatInviteLink
        {
            InviteLink = inviteLink,
            Creator = new BotApiUser { Id = bot.UserId, IsBot = true, FirstName = bot.BotName },
            IsPrimary = false,
            IsRevoked = true
        };
    }

    #endregion

    #region Join Requests

    public async Task ApproveChatJoinRequestAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();

        logger.LogInformation("Bot {BotId} approving join request from user {UserId} in chat {ChatId}",
            bot.UserId, userId, chatId);

        await mtprotoBridge.HandleChatJoinRequestAsync(bot.UserId, chatId, userId, "Approve");
    }

    public async Task DeclineChatJoinRequestAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var userId = body.GetProperty("user_id").GetInt64();

        logger.LogInformation("Bot {BotId} declining join request from user {UserId} in chat {ChatId}",
            bot.UserId, userId, chatId);

        await mtprotoBridge.HandleChatJoinRequestAsync(bot.UserId, chatId, userId, "Decline");
    }

    #endregion

    #region Chat Photo / Title / Description

    public async Task SetChatPhotoAsync(string token, long chatId, IFormFile photo)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} setting photo for chat {ChatId}", bot.UserId, chatId);

        var photoBase64 = await ConvertFormFileToBase64(photo);
        await mtprotoBridge.ManageChatPhotoAsync(bot.UserId, chatId, "Set", photoBase64);
    }

    public async Task DeleteChatPhotoAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();

        logger.LogInformation("Bot {BotId} deleting photo for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.ManageChatPhotoAsync(bot.UserId, chatId, "Delete");
    }

    public async Task SetChatTitleAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var title = body.GetProperty("title").GetString() ?? "";

        logger.LogInformation("Bot {BotId} setting title for chat {ChatId}: {Title}", bot.UserId, chatId, title);

        await mtprotoBridge.SetChatInfoAsync(bot.UserId, chatId, "SetTitle", title: title);
    }

    public async Task SetChatDescriptionAsync(string token, JsonElement body)
    {
        var bot = await GetBotByTokenAsync(token);
        var chatId = body.GetProperty("chat_id").GetInt64();
        var description = BotApiHelpers.GetOptionalString(body, "description") ?? "";

        logger.LogInformation("Bot {BotId} setting description for chat {ChatId}", bot.UserId, chatId);

        await mtprotoBridge.SetChatInfoAsync(bot.UserId, chatId, "SetDescription", description: description);
    }

    #endregion

    #region Helper Methods

    private async Task<BotReadModel> GetBotByTokenAsync(string token)
    {
        var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
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

    private static async Task<string> ConvertFormFileToBase64(IFormFile file)
    {
        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    #endregion

    #region Star Gifts

    public async Task<object> GetAvailableGiftsAsync(string token)
    {
        var bot = await GetBotByTokenAsync(token);
        logger.LogInformation("Bot {BotId} calling getAvailableGifts", bot.UserId);

        var giftsCollection = database.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var giftsReadModels = await giftsCollection.Find(_ => true).ToListAsync();

        var gifts = new List<object>();

        foreach (var gift in giftsReadModels)
        {
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

        logger.LogInformation("Bot {BotId} sending gift {GiftId} to user {UserId} (Count={Count})",
            bot.UserId, giftId, userId, count);

        var giftsCollection = database.GetCollection<AvailableStarGiftReadModel>("AvailableStarGiftReadModel");
        var gift = await giftsCollection.Find(g => g.GiftId == giftId).FirstOrDefaultAsync();

        if (gift == null)
        {
            logger.LogWarning("Gift {GiftId} not found", giftId);
            return false;
        }

        var giftEvent = new BotGiftEvent
        {
            BotUserId = bot.UserId,
            ToUserId = userId,
            GiftId = giftId,
            Count = count,
            Message = text,
            HideName = hideName,
            IncludeUpgrade = includeUpgrade,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            RandomId = Random.Shared.NextInt64()
        };

        await eventBus.PublishAsync(giftEvent);

        logger.LogInformation("Published BotGiftEvent for bot {BotId} to user {UserId}", bot.UserId, userId);
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
        var currency = body.GetProperty("currency").GetString() ?? "XTR";

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

        var invoice = new BotApiInvoice
        {
            Title = title,
            Description = description,
            StartParameter = payload,
            Currency = currency,
            TotalAmount = totalAmount
        };

        var message = new BotApiMessage
        {
            MessageId = (int)(DateTimeOffset.UtcNow.ToUnixTimeSeconds() % int.MaxValue),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = new BotApiChat { Id = chatId, Type = "private" },
            From = new BotApiUser { Id = bot.UserId, IsBot = true, FirstName = bot.BotName },
            Invoice = invoice
        };

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

        await updatesManager.AnswerPreCheckoutAsync(preCheckoutQueryId, ok, errorMessage);
        return true;
    }

    #endregion
}
