namespace MyTelegram.Domain.Shared.Business;

/// <summary>
/// Business work hours configuration
/// </summary>
public class BusinessWorkHours
{
    public string TimezoneId { get; set; } = string.Empty;
    public List<BusinessWeeklyOpen> WeeklyOpen { get; set; } = new();
    public bool OpenNow { get; set; }
}

/// <summary>
/// Weekly opening time interval
/// </summary>
public class BusinessWeeklyOpen
{
    public int StartMinute { get; set; }
    public int EndMinute { get; set; }
}

/// <summary>
/// Business location information
/// </summary>
public class BusinessLocation
{
    public string Address { get; set; } = string.Empty;
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
}

/// <summary>
/// Business greeting message configuration
/// </summary>
public class BusinessGreetingMessage
{
    public int ShortcutId { get; set; }
    public BusinessRecipients Recipients { get; set; } = new();
    public int NoActivityDays { get; set; }
}

/// <summary>
/// Business away message configuration
/// </summary>
public class BusinessAwayMessage
{
    public int ShortcutId { get; set; }
    public BusinessRecipients Recipients { get; set; } = new();
    public BusinessAwayMessageSchedule Schedule { get; set; } = new();
    public bool OfflineOnly { get; set; }
}

/// <summary>
/// Business away message schedule
/// </summary>
public class BusinessAwayMessageSchedule
{
    public BusinessAwayMessageScheduleType Type { get; set; }
    public int? StartMinute { get; set; }
    public int? EndMinute { get; set; }
}

public enum BusinessAwayMessageScheduleType
{
    Always,
    OutsideWorkHours,
    Custom
}

/// <summary>
/// Business introduction
/// </summary>
public class BusinessIntro
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? StickerDocumentId { get; set; }
}

/// <summary>
/// Business recipients configuration
/// </summary>
public class BusinessRecipients
{
    public bool ExistingChats { get; set; }
    public bool NewChats { get; set; }
    public bool Contacts { get; set; }
    public bool NonContacts { get; set; }
    public bool ExcludeSelected { get; set; }
    public List<long> Users { get; set; } = new();
}

/// <summary>
/// Business chat link
/// </summary>
public class BusinessChatLink
{
    public string Id { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public List<MessageEntity> Entities { get; set; } = new();
    public int Views { get; set; }
}

/// <summary>
/// Message entity for styled text
/// </summary>
public class MessageEntity
{
    public int Offset { get; set; }
    public int Length { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Language { get; set; }
}

/// <summary>
/// Quick reply shortcut
/// </summary>
public class QuickReply
{
    public int ShortcutId { get; set; }
    public string Shortcut { get; set; } = string.Empty;
    public int TopMessage { get; set; }
    public int Count { get; set; }
}
