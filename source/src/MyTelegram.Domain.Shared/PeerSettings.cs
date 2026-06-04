namespace MyTelegram;

public class PeerSettings
{
    // Basic contact interaction flags
    public bool AddContact { get; set; }
    public bool BlockContact { get; set; }
    public bool NeedContactsException { get; set; }
    public bool ReportGeo { get; set; }
    public bool ReportSpam { get; set; }
    public bool ShareContact { get; set; }

    // Additional action bar flags from specification
    public bool Autoarchived { get; set; }
    public bool InviteMembers { get; set; }
    public bool RequestChatBroadcast { get; set; }
    public bool BusinessBotPaused { get; set; }
    public bool BusinessBotCanReply { get; set; }

    // Optional fields
    public int? GeoDistance { get; set; }
    public string? RequestChatTitle { get; set; }
    public int? RequestChatDate { get; set; }
    public long? BusinessBotId { get; set; }
    public string? BusinessBotManageUrl { get; set; }
    public long? ChargePaidMessageStars { get; set; }
    public string? RegistrationMonth { get; set; }
    public string? PhoneCountry { get; set; }
    public int? NameChangeDate { get; set; }
    public int? PhotoChangeDate { get; set; }
}