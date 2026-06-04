using EventFlow.MongoDB.ReadStores.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace MyTelegram.ReadModel.Impl;

/// <summary>
/// ReadModel for Telegram bots - stores bot configuration and state
/// </summary>
[MongoDbCollectionName("ReadModel-BotReadModel")]
[BsonIgnoreExtraElements]
public class BotReadModel : IBotReadModel
{
    public BotReadModel()
    {
        Commands = new List<BotCommand>();
    }

    public BotReadModel(
        long userId,
        long ownerUserId,
        string token,
        string botName,
        string userName)
    {
        Id = $"bot-{userId}";
        UserId = userId;
        OwnerUserId = ownerUserId;
        Token = token;
        BotName = botName;
        UserName = userName;
        Commands = new List<BotCommand>();
        AllowAccessGroupMessages = false;
        AllowJoinGroups = true;
        InlineModeEnabled = false;
        BusinessModeEnabled = false;
        ChatAdminRights = 0;
        ChannelAdminRights = 0;
    }

    public string Id { get; set; } = default!;
    public long? Version { get; set; }
    
    /// <summary>
    /// Bot user ID in Telegram
    /// </summary>
    public long UserId { get; set; }
    
    /// <summary>
    /// Owner user ID who created the bot
    /// </summary>
    public long OwnerUserId { get; set; }
    
    /// <summary>
    /// Bot authentication token (e.g., "123456:ABC-DEF1234ghIkl-zyx57W2v1u123ew11")
    /// </summary>
    public string Token { get; set; } = default!;
    
    /// <summary>
    /// Bot display name
    /// </summary>
    public string BotName { get; set; } = default!;
    
    /// <summary>
    /// Bot username (without @)
    /// </summary>
    public string UserName { get; set; } = default!;
    
    /// <summary>
    /// Bot description (shown in bot info)
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// About text (short description)
    /// </summary>
    public string? About { get; set; }
    
    /// <summary>
    /// List of bot commands
    /// </summary>
    public List<BotCommand> Commands { get; set; }
    
    /// <summary>
    /// Webhook URL for receiving updates
    /// </summary>
    public string? WebHookUrl { get; set; }
    
    /// <summary>
    /// Secret token for webhook validation
    /// </summary>
    public string? WebhookSecretToken { get; set; }
    
    /// <summary>
    /// Maximum allowed connections for webhook
    /// </summary>
    public int? WebhookMaxConnections { get; set; }
    
    /// <summary>
    /// Allowed update types for webhook
    /// </summary>
    public List<string>? WebhookAllowedUpdates { get; set; }
    
    /// <summary>
    /// Whether bot can access all group messages
    /// </summary>
    public bool AllowAccessGroupMessages { get; set; }
    
    /// <summary>
    /// Whether bot can be added to groups
    /// </summary>
    public bool AllowJoinGroups { get; set; }
    
    /// <summary>
    /// Document ID for description media
    /// </summary>
    public long? DescriptionDocumentId { get; set; }
    
    /// <summary>
    /// Photo ID for description
    /// </summary>
    public long? DescriptionPhotoId { get; set; }
    
    /// <summary>
    /// Placeholder text for inline mode
    /// </summary>
    public string? InlinePlaceholder { get; set; }
    
    /// <summary>
    /// Whether inline mode is enabled
    /// </summary>
    public bool InlineModeEnabled { get; set; }
    
    /// <summary>
    /// Whether business mode is enabled
    /// </summary>
    public bool BusinessModeEnabled { get; set; }
    
    /// <summary>
    /// Privacy policy URL
    /// </summary>
    public string? PrivacyPolicyUrl { get; set; }
    
    /// <summary>
    /// Mini app URL
    /// </summary>
    public string? MiniAppUrl { get; set; }
    
    /// <summary>
    /// Chat admin rights flags
    /// </summary>
    public int ChatAdminRights { get; set; }
    
    /// <summary>
    /// Channel admin rights flags
    /// </summary>
    public int ChannelAdminRights { get; set; }
    
    /// <summary>
    /// Profile photo ID
    /// </summary>
    public long? ProfilePhotoId { get; set; }
    
    /// <summary>
    /// Last update ID received (for getUpdates offset)
    /// </summary>
    public long LastUpdateId { get; set; }
    
    /// <summary>
    /// Whether bot is active
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Creation timestamp
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Last activity timestamp
    /// </summary>
    public DateTime? LastActivityAt { get; set; }
}
