namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for sending media (photo/audio/document/video/etc.) via Bot API.
/// </summary>
public class BotSendMediaEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    
    /// <summary>Photo, Audio, Document, Video, Animation, Voice, VideoNote, Sticker,
    /// Location, Venue, Contact, Poll, Dice, MediaGroup</summary>
    public string MediaType { get; set; } = "";
    
    /// <summary>file_id or URL of the media</summary>
    public string? FileId { get; set; }
    
    /// <summary>Base64-encoded file data (when uploaded via multipart)</summary>
    public string? FileBase64 { get; set; }
    
    public string? Caption { get; set; }
    public string? ParseMode { get; set; }
    public bool? DisableNotification { get; set; }
    public int? ReplyToMessageId { get; set; }
    public string? ReplyMarkupJson { get; set; }
    
    // Location-specific
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    
    // Venue-specific
    public string? VenueTitle { get; set; }
    public string? VenueAddress { get; set; }
    
    // Contact-specific
    public string? PhoneNumber { get; set; }
    public string? ContactFirstName { get; set; }
    public string? ContactLastName { get; set; }
    
    // Poll-specific
    public string? Question { get; set; }
    public string? OptionsJson { get; set; }
    public bool? IsAnonymous { get; set; }
    public string? PollType { get; set; }
    
    // Dice-specific
    public string? Emoji { get; set; }
    
    public long Timestamp { get; set; }
    public long RandomId { get; set; }
}
