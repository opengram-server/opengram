namespace MyTelegram.Core;

/// <summary>
/// 
/// </summary>
/// <param name="LocKey">Notification type: a string literal in the form /[A-Z_0-9]+/, which summarizes the notification. For example, CHAT_MESSAGE_TEXT</param>
/// <param name="LocArgs">Notification placeholder arguments: a list or arguments which, when inserted into a template, produce a readable notification</param>
/// <param name="UserId">ID of the account to which the PUSH notification should be delivered, in case of clients with multiple accounts active and running</param>
/// <param name="Custom">Custom parameters to be passed into the application when a notification is opened</param>
/// <param name="Sound">The name of an audio file to be played</param>
public record PushData(string LocKey, string[] LocArgs, long UserId, PushNotificationCustomData? Custom, string? Sound);
public record PushNotificationCreatedEvent(PushData Data);

public class PushNotificationCustomData
{
    /// <summary>
    /// For notifications about media, base64url-encoded TL-serialization of the corresponding Photo / Document object
    /// </summary>
    public string? Attachb64 { get; set; }

    /// <summary>
    /// Base64url-encoded TL-serialization of the Updates object, currently sent only for PHONE_CALL_REQUEST (with updatePhoneCall inside)
    /// </summary>
    public string? Updates { get; set; }

    /// <summary>
    /// Call ID, used in PHONE_CALL_REQUEST
    /// </summary>
    public long? CallId { get; set; }

    /// <summary>
    /// Call access hash, used in PHONE_CALL_REQUEST
    /// </summary>
    public long? CallAh { get; set; }

    /// <summary>
    /// Secret chat id for ENCRYPTION_REQUEST, ENCRYPTION_ACCEPT, ENCRYPTED_MESSAGE
    /// </summary>
    public long? EncryptionId { get; set; }

    /// <summary>
    /// Random id for message in ENCRYPTED_MESSAGE
    /// </summary>
    public long? RandomId { get; set; }

    /// <summary>
    /// Telegram user identifier of contact that joined Telegram in CONTACT_JOINED
    /// </summary>
    public long? ContactId { get; set; }

    /// <summary>
    /// Message ID for new message event or reaction event
    /// </summary>
    public int? MsgId { get; set; }

    /// <summary>
    /// Identifier of the channel/supergroup where the event occurred
    /// </summary>
    public long? ChannelId { get; set; }

    /// <summary>
    /// Identifier of the basic group where the event occurred
    /// </summary>
    public long? ChatId { get; set; }

    /// <summary>
    /// User ID where the event occurred
    /// </summary>
    public long? FromId { get; set; }

    /// <summary>
    /// If the group message was sent as a channel, this field will contain the sender channel ID
    /// </summary>
    public long? ChatFromBroadcastId { get; set; }

    /// <summary>
    ///  If the group message was sent as a supergroup, this field will contain the sender supergroup ID
    /// </summary>
    public long? ChatFromGroupId { get; set; }

    /// <summary>
    /// Groups only, message author identifier (ignore if any of chat_from_broadcast_id / chat_from_group_id was present)
    /// </summary>
    public long? ChatFromId { get; set; }

    /// <summary>
    /// Whether the current user was mentioned/replied to in this new message
    /// </summary>
    public bool? Mention { get; set; }

    /// <summary>
    /// Whether the message was posted silently (no sound should be played for this notification)
    /// </summary>
    public bool? Silent { get; set; }

    /// <summary>
    /// Whether the message is outgoing and was sent via scheduled messages
    /// </summary>
    public bool? Schedule { get; set; }

    /// <summary>
    /// When was the message last edited
    /// </summary>
    public int? EditDate { get; set; }

    /// <summary>
    /// Thread_id for new mentions/replies in threads
    /// </summary>
    public int? TopMsgId { get; set; }

    /// <summary>
    /// Comma-separated IDs of messages that were deleted
    /// </summary>
    public string? Messages { get; set; }

    /// <summary>
    /// Maximum ID of read messages
    /// </summary>
    public int? MaxId { get; set; }

    /// <summary>
    /// Number of the data-center
    /// </summary>
    public int? Dc { get; set; }

    /// <summary>
    /// Server address with port number in the format 111.112.113.114:443
    /// </summary>
    public string? Addr { get; set; }
}
