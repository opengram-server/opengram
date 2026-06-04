using MyTelegram.Domain.Shared.BotApi;
using MyTelegram.Schema;
using MyTelegram.Domain.Shared.Events;
using MyTelegram.EventBus;
using System.Text.Json;

namespace MyTelegram.BotApi.Services;

/// <summary>
/// Bridge between Bot API and MTProto
/// Publishes simple events to RabbitMQ for Command Server to handle
/// </summary>
public class MTProtoBridge(
    IEventBus eventBus,
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
        
        // Serialize replyMarkup to JSON if present
        string? replyMarkupJson = null;
        if (replyMarkup != null)
        {
            replyMarkupJson = JsonSerializer.Serialize(replyMarkup, new JsonSerializerOptions 
            { 
                WriteIndented = false 
            });
        }
        
        // Create simple DTO event (NOT EventFlow domain event!)
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
        
        // Publish to RabbitMQ - Command Server will handle it
        await eventBus.PublishAsync(messageEvent);
        
        logger.LogInformation("MTProto: Published BotMessageEvent to RabbitMQ - BotId={BotId}, ChatId={ChatId}", botUserId, chatId);
        
        // Return mock response (actual message will be created by Command Server)
        return new BotApiMessage
        {
            MessageId = Random.Shared.Next(1, int.MaxValue), // Temporary ID
            Date = (int)timestamp,
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Text = text,
            From = new BotApiUser
            {
                Id = botUserId,
                IsBot = true,
                FirstName = "Bot"
            }
        };
    }
    
    /// <summary>
    /// Get messages (for getUpdates)
    /// </summary>
    public async Task<List<BotApiUpdate>> GetUpdatesAsync(long botUserId, long offset, int limit)
    {
        logger.LogDebug("MTProto: Getting updates for bot {BotId}, offset={Offset}", botUserId, offset);
        
        // TODO: Query MTProto updates from database
        // For now return empty list
        return new List<BotApiUpdate>();
    }
    
    /// <summary>
    /// Forward message through MTProto
    /// </summary>
    public async Task<BotApiMessage> ForwardMessageAsync(
        long botUserId,
        long chatId,
        long fromChatId,
        int messageId,
        bool? disableNotification = null)
    {
        logger.LogInformation("MTProto: Bot {BotId} forwarding message {MessageId} from {FromChatId} to {ChatId}",
            botUserId, messageId, fromChatId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        // TODO: Execute MTProto messages.forwardMessages
        
        return new BotApiMessage
        {
            MessageId = Random.Shared.Next(1, 1000000),
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Text = "[Forwarded]"
        };
    }
    
    /// <summary>
    /// Edit message through MTProto
    /// </summary>
    public async Task<BotApiMessage> EditMessageTextAsync(
        long botUserId,
        long chatId,
        int messageId,
        string text,
        string? parseMode = null)
    {
        logger.LogInformation("MTProto: Bot {BotId} editing message {MessageId} in chat {ChatId}",
            botUserId, messageId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        // TODO: Execute MTProto messages.editMessage
        
        return new BotApiMessage
        {
            MessageId = messageId,
            Date = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            Chat = BotApiConverter.ToBotApiChat(peerId, peerType),
            Text = text,
            EditDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };
    }
    
    /// <summary>
    /// Delete message through MTProto
    /// </summary>
    public async Task<bool> DeleteMessageAsync(long botUserId, long chatId, int messageId)
    {
        logger.LogInformation("MTProto: Bot {BotId} deleting message {MessageId} in chat {ChatId}",
            botUserId, messageId, chatId);
        
        // TODO: Execute MTProto messages.deleteMessages
        
        return true;
    }
    
    /// <summary>
    /// Get chat info through MTProto
    /// </summary>
    public async Task<BotApiChat> GetChatAsync(long botUserId, long chatId)
    {
        logger.LogInformation("MTProto: Bot {BotId} getting chat {ChatId}", botUserId, chatId);
        
        var (peerId, peerType) = BotApiConverter.FromBotApiChatId(chatId);
        
        // TODO: Query chat from database
        
        return BotApiConverter.ToBotApiChat(peerId, peerType, "Chat", null);
    }
    
    /// <summary>
    /// Send chat action through MTProto
    /// </summary>
    public async Task<bool> SendChatActionAsync(long botUserId, long chatId, string action)
    {
        logger.LogDebug("MTProto: Bot {BotId} sending action '{Action}' to chat {ChatId}",
            botUserId, action, chatId);
        
        // TODO: Execute MTProto messages.setTyping
        
        return true;
    }
}
