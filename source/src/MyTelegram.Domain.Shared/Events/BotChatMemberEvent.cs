namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for managing chat members (ban/unban/restrict/promote) via Bot API.
/// </summary>
public class BotChatMemberEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public long UserId { get; set; }
    
    /// <summary>Ban, Unban, Restrict, Promote, SetCustomTitle, BanSenderChat, UnbanSenderChat</summary>
    public string Action { get; set; } = "";
    
    // For ban
    public int? UntilDate { get; set; }
    public bool? RevokeMessages { get; set; }
    
    // For unban
    public bool? OnlyIfBanned { get; set; }
    
    // For restrict — JSON-serialized ChatPermissions
    public string? PermissionsJson { get; set; }
    public bool? UseIndependentChatPermissions { get; set; }
    
    // For promote — individual rights
    public bool? IsAnonymous { get; set; }
    public bool? CanManageChat { get; set; }
    public bool? CanDeleteMessages { get; set; }
    public bool? CanManageVideoChats { get; set; }
    public bool? CanRestrictMembers { get; set; }
    public bool? CanPromoteMembers { get; set; }
    public bool? CanChangeInfo { get; set; }
    public bool? CanInviteUsers { get; set; }
    public bool? CanPostStories { get; set; }
    public bool? CanEditStories { get; set; }
    public bool? CanDeleteStories { get; set; }
    public bool? CanPostMessages { get; set; }
    public bool? CanEditMessages { get; set; }
    public bool? CanPinMessages { get; set; }
    public bool? CanManageTopics { get; set; }
    
    // For custom title
    public string? CustomTitle { get; set; }
    
    // For sender chat ban/unban
    public long? SenderChatId { get; set; }
    
    public long Timestamp { get; set; }
}
