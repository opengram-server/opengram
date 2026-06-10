namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for managing chat invite links via Bot API.
/// </summary>
public class BotChatInviteLinkEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    
    /// <summary>Export, Create, Edit, Revoke</summary>
    public string Action { get; set; } = "";
    
    // For create/edit
    public string? InviteLink { get; set; }
    public string? Name { get; set; }
    public int? ExpireDate { get; set; }
    public int? MemberLimit { get; set; }
    public bool? CreatesJoinRequest { get; set; }
    
    public long Timestamp { get; set; }
}
