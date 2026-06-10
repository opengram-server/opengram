using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.EventBus;
using MyTelegram.ReadModel.Impl;
using MongoDB.Driver;
using System.Text.Json;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Bridge between Bot API and MTProto.
/// Publishes DTO events to RabbitMQ for Command Server to handle.
/// Queries MongoDB for read-only data.
/// </summary>
public class MTProtoBridge(
    IEventBus eventBus,
    IMongoDatabase database,
    ILogger<MTProtoBridge> logger)
{
    /// <summary>
    /// Send message through MTProto
    /// </summary>
    public async Task<BotApiMessage> SendMessageAsync(
        long botUserId, 
        long chatId, 
        string text,
        string? parseMode = null,
        bool? disableWebPagePreview = null,
        bool? disableNotification = null,
        int? replyToMessageId = null,
        IReplyMarkup? replyMarkup = null)
    {
        logger.LogInformation("MTProto: Bot {BotId} sending message to {ChatId} via RabbitMQ", botUserId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        var randomId = Random.Shared.NextInt64();
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        string? replyMarkupJson = null;
        if (replyMarkup != null)
        {
            replyMarkupJson = JsonSerializer.Serialize(replyMarkup, new JsonSerializerOptions 
            { 
                WriteIndented = false 
            });
        }
        
        var messageEvent = new BotMessageEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            PeerId = peerId,
            PeerType = peerType,
            Text = text,
            ParseMode = parseMode,
            DisableWebPagePreview = disableWebPagePreview,
            DisableNotification = disableNotification,
            ReplyToMessageId = replyToMessageId,
            ReplyMarkupJson = replyMarkupJson,
            Timestamp = timestamp,
            RandomId = randomId
        };
        
        await eventBus.PublishAsync(messageEvent);
        
        logger.LogInformation("MTProto: Published BotMessageEvent - BotId={BotId}, ChatId={ChatId}", botUserId, chatId);
        
        var bot = await GetBotInfoAsync(botUserId);
        
        return new BotApiMessage
        {
            MessageId = Random.Shared.Next(1, int.MaxValue),
            Date = (int)timestamp,
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Text = text,
            From = new BotApiUser
            {
                Id = botUserId,
                IsBot = true,
                FirstName = bot?.BotName ?? "Bot"
            }
        };
    }
    
    /// <summary>
    /// Get messages (for getUpdates)
    /// </summary>
    public Task<List<BotApiUpdate>> GetUpdatesAsync(long botUserId, long offset, int limit)
    {
        logger.LogDebug("MTProto: Getting updates for bot {BotId}, offset={Offset}", botUserId, offset);
        return Task.FromResult(new List<BotApiUpdate>());
    }
    
    /// <summary>
    /// Forward message through MTProto
    /// </summary>
    public async Task<BotApiMessage> ForwardMessageAsync(
        long botUserId,
        long chatId,
        long fromChatId,
        int messageId,
        bool disableNotification = false,
        bool protectContent = false)
    {
        logger.LogInformation("MTProto: Bot {BotId} forwarding message {MessageId} from {FromChatId} to {ChatId}",
            botUserId, messageId, fromChatId, chatId);
        
        var (toPeerId, toPeerType) = BotApiConverter.FromBotApiChatId(chatId);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        var forwardEvent = new BotForwardMessageEvent
        {
            BotUserId = botUserId,
            ToChatId = chatId,
            FromChatId = fromChatId,
            MessageId = messageId,
            DisableNotification = disableNotification,
            ProtectContent = protectContent,
            Timestamp = timestamp,
            RandomId = Random.Shared.NextInt64()
        };
        
        await eventBus.PublishAsync(forwardEvent);
        
        logger.LogInformation("MTProto: Published BotForwardMessageEvent - BotId={BotId}", botUserId);
        
        // Try to get the original message from MongoDB to populate the response
        var originalMessage = await GetMessageFromDbAsync(fromChatId, messageId);
        
        return new BotApiMessage
        {
            MessageId = Random.Shared.Next(1, int.MaxValue),
            Date = (int)timestamp,
            Chat = BotApiConverter.ToBotApiChat(toPeerId, toPeerType),
            Text = originalMessage?.Text,
            From = await GetBotApiUserAsync(botUserId)
        };
    }
    
    /// <summary>
    /// Edit message text through MTProto
    /// </summary>
    public async Task<BotApiMessage> EditMessageTextAsync(
        long botUserId,
        long chatId,
        int messageId,
        string text,
        string? parseMode = null,
        string? replyMarkupJson = null)
    {
        logger.LogInformation("MTProto: Bot {BotId} editing message {MessageId} in chat {ChatId}",
            botUserId, messageId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        var editEvent = new BotEditMessageEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            MessageId = messageId,
            Text = text,
            ParseMode = parseMode,
            ReplyMarkupJson = replyMarkupJson,
            EditText = true,
            EditReplyMarkup = replyMarkupJson != null,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(editEvent);
        
        logger.LogInformation("MTProto: Published BotEditMessageEvent - BotId={BotId}", botUserId);
        
        return new BotApiMessage
        {
            MessageId = messageId,
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Text = text,
            EditDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            From = await GetBotApiUserAsync(botUserId)
        };
    }
    
    /// <summary>
    /// Edit message reply markup through MTProto
    /// </summary>
    public async Task<BotApiMessage> EditMessageReplyMarkupAsync(
        long botUserId,
        long chatId,
        int messageId,
        string? replyMarkupJson = null)
    {
        logger.LogInformation("MTProto: Bot {BotId} editing reply markup for message {MessageId} in chat {ChatId}",
            botUserId, messageId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        var editEvent = new BotEditMessageEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            MessageId = messageId,
            ReplyMarkupJson = replyMarkupJson,
            EditText = false,
            EditReplyMarkup = true,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(editEvent);
        
        return new BotApiMessage
        {
            MessageId = messageId,
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            EditDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            From = await GetBotApiUserAsync(botUserId)
        };
    }
    
    /// <summary>
    /// Delete message through MTProto
    /// </summary>
    public async Task<bool> DeleteMessageAsync(long botUserId, long chatId, int messageId)
    {
        logger.LogInformation("MTProto: Bot {BotId} deleting message {MessageId} in chat {ChatId}",
            botUserId, messageId, chatId);
        
        var deleteEvent = new BotDeleteMessageEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            MessageId = messageId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(deleteEvent);
        
        logger.LogInformation("MTProto: Published BotDeleteMessageEvent - BotId={BotId}", botUserId);
        
        return true;
    }
    
    /// <summary>
    /// Get chat info through MTProto — queries MongoDB ChannelReadModel / ChatReadModel / UserReadModel
    /// </summary>
    public async Task<BotApiChat> GetChatAsync(long botUserId, long chatId)
    {
        logger.LogInformation("MTProto: Bot {BotId} getting chat {ChatId}", botUserId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        switch (peerType)
        {
            case PeerType.User:
            {
                var usersCollection = database.GetCollection<UserReadModel>("ReadModel-UserReadModel");
                var user = await usersCollection.Find(u => u.UserId == peerId).FirstOrDefaultAsync();
                if (user != null)
                {
                    return new BotApiChat
                    {
                        Id = chatId,
                        Type = "private",
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Username = user.UserName
                    };
                }
                break;
            }
            case PeerType.Chat:
            {
                var chatsCollection = database.GetCollection<ChatReadModel>("ReadModel-ChatReadModel");
                var chat = await chatsCollection.Find(c => c.ChatId == peerId).FirstOrDefaultAsync();
                if (chat != null)
                {
                    return new BotApiChat
                    {
                        Id = chatId,
                        Type = "group",
                        Title = chat.Title
                    };
                }
                break;
            }
            case PeerType.Channel:
            {
                var channelsCollection = database.GetCollection<ChannelReadModel>("ReadModel-ChannelReadModel");
                var channel = await channelsCollection.Find(c => c.ChannelId == peerId).FirstOrDefaultAsync();
                if (channel != null)
                {
                    return new BotApiChat
                    {
                        Id = chatId,
                        Type = channel.Broadcast ? "channel" : "supergroup",
                        Title = channel.Title,
                        Username = channel.UserName
                    };
                }
                break;
            }
        }
        
        return BotApiConverter.ToBotApiChat(peerId, peerType, "Chat", null);
    }
    
    /// <summary>
    /// Send chat action through MTProto
    /// </summary>
    public async Task<bool> SendChatActionAsync(long botUserId, long chatId, string action, int? messageThreadId = null)
    {
        logger.LogDebug("MTProto: Bot {BotId} sending action '{Action}' to chat {ChatId}",
            botUserId, action, chatId);
        
        var chatActionEvent = new BotChatActionEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            Action = action,
            MessageThreadId = messageThreadId,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(chatActionEvent);
        
        return true;
    }
    
    /// <summary>
    /// Answer callback query
    /// </summary>
    public async Task<bool> AnswerCallbackQueryAsync(
        long botUserId,
        long queryId,
        string? text = null,
        bool showAlert = false,
        string? url = null,
        int cacheTime = 0)
    {
        logger.LogInformation("MTProto: Bot {BotId} answering callback query {QueryId}", botUserId, queryId);
        
        var answerEvent = new BotCallbackQueryAnswerEvent
        {
            BotUserId = botUserId,
            QueryId = queryId,
            Text = text,
            ShowAlert = showAlert,
            Url = url,
            CacheTime = cacheTime,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(answerEvent);
        
        return true;
    }
    
    /// <summary>
    /// Manage chat member (ban/unban/restrict/promote/etc.)
    /// </summary>
    public async Task ManageChatMemberAsync(BotChatMemberEvent memberEvent)
    {
        logger.LogInformation("MTProto: Bot {BotId} {Action} user {UserId} in chat {ChatId}",
            memberEvent.BotUserId, memberEvent.Action, memberEvent.UserId, memberEvent.ChatId);
        
        await eventBus.PublishAsync(memberEvent);
    }
    
    /// <summary>
    /// Set default chat permissions
    /// </summary>
    public async Task SetChatPermissionsAsync(long botUserId, long chatId, string permissionsJson, bool useIndependent = false)
    {
        var permissionsEvent = new BotChatPermissionsEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            PermissionsJson = permissionsJson,
            UseIndependentChatPermissions = useIndependent,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(permissionsEvent);
    }
    
    /// <summary>
    /// Manage chat invite link
    /// </summary>
    public async Task ManageChatInviteLinkAsync(BotChatInviteLinkEvent inviteLinkEvent)
    {
        logger.LogInformation("MTProto: Bot {BotId} {Action} invite link for chat {ChatId}",
            inviteLinkEvent.BotUserId, inviteLinkEvent.Action, inviteLinkEvent.ChatId);
        
        await eventBus.PublishAsync(inviteLinkEvent);
    }
    
    /// <summary>
    /// Handle chat join request (approve/decline)
    /// </summary>
    public async Task HandleChatJoinRequestAsync(long botUserId, long chatId, long userId, string action)
    {
        var joinRequestEvent = new BotChatJoinRequestEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            UserId = userId,
            Action = action,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(joinRequestEvent);
    }
    
    /// <summary>
    /// Set/delete chat photo
    /// </summary>
    public async Task ManageChatPhotoAsync(long botUserId, long chatId, string action, string? photoBase64 = null)
    {
        var photoEvent = new BotChatPhotoEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            Action = action,
            PhotoBase64 = photoBase64,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(photoEvent);
    }
    
    /// <summary>
    /// Set chat title or description
    /// </summary>
    public async Task SetChatInfoAsync(long botUserId, long chatId, string action, string? title = null, string? description = null)
    {
        var infoEvent = new BotChatInfoEvent
        {
            BotUserId = botUserId,
            ChatId = chatId,
            Action = action,
            Title = title,
            Description = description,
            Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
        
        await eventBus.PublishAsync(infoEvent);
    }
    
    /// <summary>
    /// Send media message
    /// </summary>
    public async Task<BotApiMessage> SendMediaAsync(BotSendMediaEvent mediaEvent)
    {
        logger.LogInformation("MTProto: Bot {BotId} sending {MediaType} to chat {ChatId}",
            mediaEvent.BotUserId, mediaEvent.MediaType, mediaEvent.ChatId);
        
        mediaEvent.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        mediaEvent.RandomId = Random.Shared.NextInt64();
        
        await eventBus.PublishAsync(mediaEvent);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(mediaEvent.ChatId);
        
        return new BotApiMessage
        {
            MessageId = Random.Shared.Next(1, int.MaxValue),
            Date = (int)mediaEvent.Timestamp,
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Caption = mediaEvent.Caption,
            From = await GetBotApiUserAsync(mediaEvent.BotUserId)
        };
    }
    
    #region Helper Methods
    
    private async Task<BotApiUser> GetBotApiUserAsync(long botUserId)
    {
        var bot = await GetBotInfoAsync(botUserId);
        return new BotApiUser
        {
            Id = botUserId,
            IsBot = true,
            FirstName = bot?.BotName ?? "Bot"
        };
    }
    
    private async Task<BotReadModel?> GetBotInfoAsync(long botUserId)
    {
        try
        {
            var botsCollection = database.GetCollection<BotReadModel>("ReadModel-BotReadModel");
            return await botsCollection.Find(b => b.UserId == botUserId).FirstOrDefaultAsync();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to get bot info for {BotId}", botUserId);
            return null;
        }
    }
    
    private async Task<BotApiMessage?> GetMessageFromDbAsync(long chatId, int messageId)
    {
        try
        {
            var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
            var messagesCollection = database.GetCollection<MessageReadModel>("ReadModel-MessageReadModel");
            var message = await messagesCollection
                .Find(m => m.OwnerPeerId == peerId && m.MessageId == messageId)
                .FirstOrDefaultAsync();
            
            if (message != null)
            {
                return new BotApiMessage
                {
                    MessageId = message.MessageId,
                    Date = message.Date,
                    Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
                    Text = message.Message
                };
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to load message {MessageId} from chat {ChatId}", messageId, chatId);
        }
        
        return null;
    }
    
    #endregion
}
