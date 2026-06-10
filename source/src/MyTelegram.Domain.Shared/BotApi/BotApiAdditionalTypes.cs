using System.Text.Json.Serialization;

namespace MyTelegram.Domain.Shared.BotApi;

/// <summary>
/// Bot API File type
/// </summary>
public class BotApiFile
{
    [JsonPropertyName("file_id")]
    public string FileId { get; set; } = default!;

    [JsonPropertyName("file_unique_id")]
    public string FileUniqueId { get; set; } = default!;

    [JsonPropertyName("file_size")]
    public long? FileSize { get; set; }

    [JsonPropertyName("file_path")]
    public string? FilePath { get; set; }
}

/// <summary>
/// Bot API UserProfilePhotos type
/// </summary>
public class BotApiUserProfilePhotos
{
    [JsonPropertyName("total_count")]
    public int TotalCount { get; set; }

    [JsonPropertyName("photos")]
    public List<List<BotApiPhotoSize>> Photos { get; set; } = new();
}

/// <summary>
/// Bot API PhotoSize type
/// </summary>
public class BotApiPhotoSize
{
    [JsonPropertyName("file_id")]
    public string FileId { get; set; } = default!;

    [JsonPropertyName("file_unique_id")]
    public string FileUniqueId { get; set; } = default!;

    [JsonPropertyName("width")]
    public int Width { get; set; }

    [JsonPropertyName("height")]
    public int Height { get; set; }

    [JsonPropertyName("file_size")]
    public long? FileSize { get; set; }
}

/// <summary>
/// Bot API ChatInviteLink type
/// </summary>
public class BotApiChatInviteLink
{
    [JsonPropertyName("invite_link")]
    public string InviteLink { get; set; } = default!;

    [JsonPropertyName("creator")]
    public BotApiUser Creator { get; set; } = default!;

    [JsonPropertyName("creates_join_request")]
    public bool CreatesJoinRequest { get; set; }

    [JsonPropertyName("is_primary")]
    public bool IsPrimary { get; set; }

    [JsonPropertyName("is_revoked")]
    public bool IsRevoked { get; set; }

    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("expire_date")]
    public int? ExpireDate { get; set; }

    [JsonPropertyName("member_limit")]
    public int? MemberLimit { get; set; }

    [JsonPropertyName("pending_join_request_count")]
    public int? PendingJoinRequestCount { get; set; }
}

/// <summary>
/// Bot API ChatPermissions type
/// </summary>
public class BotApiChatPermissions
{
    [JsonPropertyName("can_send_messages")]
    public bool? CanSendMessages { get; set; }

    [JsonPropertyName("can_send_audios")]
    public bool? CanSendAudios { get; set; }

    [JsonPropertyName("can_send_documents")]
    public bool? CanSendDocuments { get; set; }

    [JsonPropertyName("can_send_photos")]
    public bool? CanSendPhotos { get; set; }

    [JsonPropertyName("can_send_videos")]
    public bool? CanSendVideos { get; set; }

    [JsonPropertyName("can_send_video_notes")]
    public bool? CanSendVideoNotes { get; set; }

    [JsonPropertyName("can_send_voice_notes")]
    public bool? CanSendVoiceNotes { get; set; }

    [JsonPropertyName("can_send_polls")]
    public bool? CanSendPolls { get; set; }

    [JsonPropertyName("can_send_other_messages")]
    public bool? CanSendOtherMessages { get; set; }

    [JsonPropertyName("can_add_web_page_previews")]
    public bool? CanAddWebPagePreviews { get; set; }

    [JsonPropertyName("can_change_info")]
    public bool? CanChangeInfo { get; set; }

    [JsonPropertyName("can_invite_users")]
    public bool? CanInviteUsers { get; set; }

    [JsonPropertyName("can_pin_messages")]
    public bool? CanPinMessages { get; set; }

    [JsonPropertyName("can_manage_topics")]
    public bool? CanManageTopics { get; set; }
}

/// <summary>
/// Bot API Location type
/// </summary>
public class BotApiLocation
{
    [JsonPropertyName("longitude")]
    public double Longitude { get; set; }

    [JsonPropertyName("latitude")]
    public double Latitude { get; set; }

    [JsonPropertyName("horizontal_accuracy")]
    public double? HorizontalAccuracy { get; set; }
}

/// <summary>
/// Bot API Contact type
/// </summary>
public class BotApiContact
{
    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = default!;

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = default!;

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("user_id")]
    public long? UserId { get; set; }

    [JsonPropertyName("vcard")]
    public string? Vcard { get; set; }
}

/// <summary>
/// Bot API Venue type
/// </summary>
public class BotApiVenue
{
    [JsonPropertyName("location")]
    public BotApiLocation Location { get; set; } = default!;

    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("address")]
    public string Address { get; set; } = default!;

    [JsonPropertyName("foursquare_id")]
    public string? FoursquareId { get; set; }

    [JsonPropertyName("foursquare_type")]
    public string? FoursquareType { get; set; }
}

/// <summary>
/// Bot API Dice type
/// </summary>
public class BotApiDice
{
    [JsonPropertyName("emoji")]
    public string Emoji { get; set; } = "🎲";

    [JsonPropertyName("value")]
    public int Value { get; set; }
}

/// <summary>
/// Bot API Poll type
/// </summary>
public class BotApiPoll
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("question")]
    public string Question { get; set; } = default!;

    [JsonPropertyName("options")]
    public List<BotApiPollOption> Options { get; set; } = new();

    [JsonPropertyName("total_voter_count")]
    public int TotalVoterCount { get; set; }

    [JsonPropertyName("is_closed")]
    public bool IsClosed { get; set; }

    [JsonPropertyName("is_anonymous")]
    public bool IsAnonymous { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "regular";
}

/// <summary>
/// Bot API PollOption type
/// </summary>
public class BotApiPollOption
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = default!;

    [JsonPropertyName("voter_count")]
    public int VoterCount { get; set; }
}

/// <summary>
/// Bot API MessageId type (for copyMessage)
/// </summary>
public class BotApiMessageId
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }
}
