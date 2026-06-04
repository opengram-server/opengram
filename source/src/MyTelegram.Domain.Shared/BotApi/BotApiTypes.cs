using System.Text.Json.Serialization;

namespace MyTelegram.Domain.Shared.BotApi;

/// <summary>
/// Bot API response wrapper
/// </summary>
public class BotApiResponse<T>
{
    [JsonPropertyName("ok")]
    public bool Ok { get; set; }

    [JsonPropertyName("result")]
    public T? Result { get; set; }

    [JsonPropertyName("error_code")]
    public int? ErrorCode { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    public static BotApiResponse<T> Success(T result) => new()
    {
        Ok = true,
        Result = result
    };

    public static BotApiResponse<T> Error(int errorCode, string description) => new()
    {
        Ok = false,
        ErrorCode = errorCode,
        Description = description
    };
}

/// <summary>
/// Bot API User type
/// </summary>
public class BotApiUser
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("is_bot")]
    public bool IsBot { get; set; }

    [JsonPropertyName("first_name")]
    public string FirstName { get; set; } = default!;

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("language_code")]
    public string? LanguageCode { get; set; }

    [JsonPropertyName("is_premium")]
    public bool? IsPremium { get; set; }

    [JsonPropertyName("added_to_attachment_menu")]
    public bool? AddedToAttachmentMenu { get; set; }

    [JsonPropertyName("can_join_groups")]
    public bool? CanJoinGroups { get; set; }

    [JsonPropertyName("can_read_all_group_messages")]
    public bool? CanReadAllGroupMessages { get; set; }

    [JsonPropertyName("supports_inline_queries")]
    public bool? SupportsInlineQueries { get; set; }

    [JsonPropertyName("can_connect_to_business")]
    public bool? CanConnectToBusiness { get; set; }

    [JsonPropertyName("has_main_web_app")]
    public bool? HasMainWebApp { get; set; }
}

/// <summary>
/// Bot API Chat type
/// </summary>
public class BotApiChat
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = default!; // "private", "group", "supergroup", "channel"

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    [JsonPropertyName("username")]
    public string? Username { get; set; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; set; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; set; }

    [JsonPropertyName("is_forum")]
    public bool? IsForum { get; set; }
}

/// <summary>
/// Bot API Message type
/// </summary>
public class BotApiMessage
{
    [JsonPropertyName("message_id")]
    public int MessageId { get; set; }

    [JsonPropertyName("message_thread_id")]
    public int? MessageThreadId { get; set; }

    [JsonPropertyName("from")]
    public BotApiUser? From { get; set; }

    [JsonPropertyName("sender_chat")]
    public BotApiChat? SenderChat { get; set; }

    [JsonPropertyName("sender_boost_count")]
    public int? SenderBoostCount { get; set; }

    [JsonPropertyName("sender_business_bot")]
    public BotApiUser? SenderBusinessBot { get; set; }

    [JsonPropertyName("date")]
    public int Date { get; set; }

    [JsonPropertyName("business_connection_id")]
    public string? BusinessConnectionId { get; set; }

    [JsonPropertyName("chat")]
    public BotApiChat Chat { get; set; } = default!;

    [JsonPropertyName("forward_origin")]
    public object? ForwardOrigin { get; set; }

    [JsonPropertyName("is_topic_message")]
    public bool? IsTopicMessage { get; set; }

    [JsonPropertyName("is_automatic_forward")]
    public bool? IsAutomaticForward { get; set; }

    [JsonPropertyName("reply_to_message")]
    public BotApiMessage? ReplyToMessage { get; set; }

    [JsonPropertyName("external_reply")]
    public object? ExternalReply { get; set; }

    [JsonPropertyName("quote")]
    public object? Quote { get; set; }

    [JsonPropertyName("reply_to_story")]
    public object? ReplyToStory { get; set; }

    [JsonPropertyName("via_bot")]
    public BotApiUser? ViaBot { get; set; }

    [JsonPropertyName("edit_date")]
    public int? EditDate { get; set; }

    [JsonPropertyName("has_protected_content")]
    public bool? HasProtectedContent { get; set; }

    [JsonPropertyName("is_from_offline")]
    public bool? IsFromOffline { get; set; }

    [JsonPropertyName("media_group_id")]
    public string? MediaGroupId { get; set; }

    [JsonPropertyName("author_signature")]
    public string? AuthorSignature { get; set; }

    [JsonPropertyName("text")]
    public string? Text { get; set; }

    [JsonPropertyName("entities")]
    public List<BotApiMessageEntity>? Entities { get; set; }

    [JsonPropertyName("link_preview_options")]
    public object? LinkPreviewOptions { get; set; }

    [JsonPropertyName("effect_id")]
    public string? EffectId { get; set; }

    [JsonPropertyName("animation")]
    public object? Animation { get; set; }

    [JsonPropertyName("audio")]
    public object? Audio { get; set; }

    [JsonPropertyName("document")]
    public object? Document { get; set; }

    [JsonPropertyName("paid_media")]
    public object? PaidMedia { get; set; }

    [JsonPropertyName("photo")]
    public List<object>? Photo { get; set; }

    [JsonPropertyName("sticker")]
    public object? Sticker { get; set; }

    [JsonPropertyName("story")]
    public object? Story { get; set; }

    [JsonPropertyName("video")]
    public object? Video { get; set; }

    [JsonPropertyName("video_note")]
    public object? VideoNote { get; set; }

    [JsonPropertyName("voice")]
    public object? Voice { get; set; }

    [JsonPropertyName("caption")]
    public string? Caption { get; set; }

    [JsonPropertyName("caption_entities")]
    public List<BotApiMessageEntity>? CaptionEntities { get; set; }

    [JsonPropertyName("show_caption_above_media")]
    public bool? ShowCaptionAboveMedia { get; set; }

    [JsonPropertyName("has_media_spoiler")]
    public bool? HasMediaSpoiler { get; set; }

    [JsonPropertyName("contact")]
    public object? Contact { get; set; }

    [JsonPropertyName("dice")]
    public object? Dice { get; set; }

    [JsonPropertyName("game")]
    public object? Game { get; set; }

    [JsonPropertyName("poll")]
    public object? Poll { get; set; }

    [JsonPropertyName("venue")]
    public object? Venue { get; set; }

    [JsonPropertyName("location")]
    public object? Location { get; set; }

    [JsonPropertyName("new_chat_members")]
    public List<BotApiUser>? NewChatMembers { get; set; }

    [JsonPropertyName("left_chat_member")]
    public BotApiUser? LeftChatMember { get; set; }

    [JsonPropertyName("new_chat_title")]
    public string? NewChatTitle { get; set; }

    [JsonPropertyName("new_chat_photo")]
    public List<object>? NewChatPhoto { get; set; }

    [JsonPropertyName("delete_chat_photo")]
    public bool? DeleteChatPhoto { get; set; }

    [JsonPropertyName("group_chat_created")]
    public bool? GroupChatCreated { get; set; }

    [JsonPropertyName("supergroup_chat_created")]
    public bool? SupergroupChatCreated { get; set; }

    [JsonPropertyName("channel_chat_created")]
    public bool? ChannelChatCreated { get; set; }

    [JsonPropertyName("message_auto_delete_timer_changed")]
    public object? MessageAutoDeleteTimerChanged { get; set; }

    [JsonPropertyName("migrate_to_chat_id")]
    public long? MigrateToChatId { get; set; }

    [JsonPropertyName("migrate_from_chat_id")]
    public long? MigrateFromChatId { get; set; }

    [JsonPropertyName("pinned_message")]
    public object? PinnedMessage { get; set; }

    [JsonPropertyName("invoice")]
    public BotApiInvoice? Invoice { get; set; }

    [JsonPropertyName("successful_payment")]
    public BotApiSuccessfulPayment? SuccessfulPayment { get; set; }

    [JsonPropertyName("refunded_payment")]
    public object? RefundedPayment { get; set; }

    [JsonPropertyName("users_shared")]
    public object? UsersShared { get; set; }

    [JsonPropertyName("chat_shared")]
    public object? ChatShared { get; set; }

    [JsonPropertyName("connected_website")]
    public string? ConnectedWebsite { get; set; }

    [JsonPropertyName("write_access_allowed")]
    public object? WriteAccessAllowed { get; set; }

    [JsonPropertyName("passport_data")]
    public object? PassportData { get; set; }

    [JsonPropertyName("proximity_alert_triggered")]
    public object? ProximityAlertTriggered { get; set; }

    [JsonPropertyName("boost_added")]
    public object? BoostAdded { get; set; }

    [JsonPropertyName("chat_background_set")]
    public object? ChatBackgroundSet { get; set; }

    [JsonPropertyName("forum_topic_created")]
    public object? ForumTopicCreated { get; set; }

    [JsonPropertyName("forum_topic_edited")]
    public object? ForumTopicEdited { get; set; }

    [JsonPropertyName("forum_topic_closed")]
    public object? ForumTopicClosed { get; set; }

    [JsonPropertyName("forum_topic_reopened")]
    public object? ForumTopicReopened { get; set; }

    [JsonPropertyName("general_forum_topic_hidden")]
    public object? GeneralForumTopicHidden { get; set; }

    [JsonPropertyName("general_forum_topic_unhidden")]
    public object? GeneralForumTopicUnhidden { get; set; }

    [JsonPropertyName("giveaway_created")]
    public object? GiveawayCreated { get; set; }

    [JsonPropertyName("giveaway")]
    public object? Giveaway { get; set; }

    [JsonPropertyName("giveaway_winners")]
    public object? GiveawayWinners { get; set; }

    [JsonPropertyName("giveaway_completed")]
    public object? GiveawayCompleted { get; set; }

    [JsonPropertyName("video_chat_scheduled")]
    public object? VideoChatScheduled { get; set; }

    [JsonPropertyName("video_chat_started")]
    public object? VideoChatStarted { get; set; }

    [JsonPropertyName("video_chat_ended")]
    public object? VideoChatEnded { get; set; }

    [JsonPropertyName("video_chat_participants_invited")]
    public object? VideoChatParticipantsInvited { get; set; }

    [JsonPropertyName("web_app_data")]
    public object? WebAppData { get; set; }

    [JsonPropertyName("reply_markup")]
    public object? ReplyMarkup { get; set; }
}

/// <summary>
/// Bot API MessageEntity type
/// </summary>
public class BotApiMessageEntity
{
    [JsonPropertyName("type")]
    public string Type { get; set; } = default!;

    [JsonPropertyName("offset")]
    public int Offset { get; set; }

    [JsonPropertyName("length")]
    public int Length { get; set; }

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("user")]
    public BotApiUser? User { get; set; }

    [JsonPropertyName("language")]
    public string? Language { get; set; }

    [JsonPropertyName("custom_emoji_id")]
    public string? CustomEmojiId { get; set; }
}

/// <summary>
/// Bot API Update type
/// </summary>
public class BotApiUpdate
{
    [JsonPropertyName("update_id")]
    public long UpdateId { get; set; }

    [JsonPropertyName("message")]
    public BotApiMessage? Message { get; set; }

    [JsonPropertyName("edited_message")]
    public BotApiMessage? EditedMessage { get; set; }

    [JsonPropertyName("channel_post")]
    public BotApiMessage? ChannelPost { get; set; }

    [JsonPropertyName("edited_channel_post")]
    public BotApiMessage? EditedChannelPost { get; set; }

    [JsonPropertyName("business_connection")]
    public object? BusinessConnection { get; set; }

    [JsonPropertyName("business_message")]
    public BotApiMessage? BusinessMessage { get; set; }

    [JsonPropertyName("edited_business_message")]
    public BotApiMessage? EditedBusinessMessage { get; set; }

    [JsonPropertyName("deleted_business_messages")]
    public object? DeletedBusinessMessages { get; set; }

    [JsonPropertyName("message_reaction")]
    public object? MessageReaction { get; set; }

    [JsonPropertyName("message_reaction_count")]
    public object? MessageReactionCount { get; set; }

    [JsonPropertyName("inline_query")]
    public object? InlineQuery { get; set; }

    [JsonPropertyName("chosen_inline_result")]
    public object? ChosenInlineResult { get; set; }

    [JsonPropertyName("callback_query")]
    public BotApiCallbackQuery? CallbackQuery { get; set; }

    [JsonPropertyName("shipping_query")]
    public object? ShippingQuery { get; set; }

    [JsonPropertyName("pre_checkout_query")]
    public BotApiPreCheckoutQuery? PreCheckoutQuery { get; set; }

    [JsonPropertyName("purchased_paid_media")]
    public object? PurchasedPaidMedia { get; set; }

    [JsonPropertyName("poll")]
    public object? Poll { get; set; }

    [JsonPropertyName("poll_answer")]
    public object? PollAnswer { get; set; }

    [JsonPropertyName("my_chat_member")]
    public object? MyChatMember { get; set; }

    [JsonPropertyName("chat_member")]
    public object? ChatMember { get; set; }

    [JsonPropertyName("chat_join_request")]
    public object? ChatJoinRequest { get; set; }

    [JsonPropertyName("chat_boost")]
    public object? ChatBoost { get; set; }

    [JsonPropertyName("removed_chat_boost")]
    public object? RemovedChatBoost { get; set; }
}

/// <summary>
/// Bot API CallbackQuery type
/// </summary>
public class BotApiCallbackQuery
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("from")]
    public BotApiUser From { get; set; } = default!;

    [JsonPropertyName("message")]
    public object? Message { get; set; }

    [JsonPropertyName("inline_message_id")]
    public string? InlineMessageId { get; set; }

    [JsonPropertyName("chat_instance")]
    public string ChatInstance { get; set; } = default!;

    [JsonPropertyName("data")]
    public string? Data { get; set; }

    [JsonPropertyName("game_short_name")]
    public string? GameShortName { get; set; }
}

/// <summary>
/// Bot API WebhookInfo type
/// </summary>
public class BotApiWebhookInfo
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = default!;

    [JsonPropertyName("has_custom_certificate")]
    public bool HasCustomCertificate { get; set; }

    [JsonPropertyName("pending_update_count")]
    public int PendingUpdateCount { get; set; }

    [JsonPropertyName("ip_address")]
    public string? IpAddress { get; set; }

    [JsonPropertyName("last_error_date")]
    public int? LastErrorDate { get; set; }

    [JsonPropertyName("last_error_message")]
    public string? LastErrorMessage { get; set; }

    [JsonPropertyName("last_synchronization_error_date")]
    public int? LastSynchronizationErrorDate { get; set; }

    [JsonPropertyName("max_connections")]
    public int? MaxConnections { get; set; }

    [JsonPropertyName("allowed_updates")]
    public List<string>? AllowedUpdates { get; set; }
}

/// <summary>
/// Bot API LabeledPrice type for invoices
/// </summary>
public class BotApiLabeledPrice
{
    [JsonPropertyName("label")]
    public string Label { get; set; } = default!;

    [JsonPropertyName("amount")]
    public int Amount { get; set; }
}

/// <summary>
/// Bot API Invoice type
/// </summary>
public class BotApiInvoice
{
    [JsonPropertyName("title")]
    public string Title { get; set; } = default!;

    [JsonPropertyName("description")]
    public string Description { get; set; } = default!;

    [JsonPropertyName("start_parameter")]
    public string StartParameter { get; set; } = default!;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = default!;

    [JsonPropertyName("total_amount")]
    public int TotalAmount { get; set; }
}

/// <summary>
/// Bot API SuccessfulPayment type
/// </summary>
public class BotApiSuccessfulPayment
{
    [JsonPropertyName("currency")]
    public string Currency { get; set; } = default!;

    [JsonPropertyName("total_amount")]
    public int TotalAmount { get; set; }

    [JsonPropertyName("invoice_payload")]
    public string InvoicePayload { get; set; } = default!;

    [JsonPropertyName("telegram_payment_charge_id")]
    public string TelegramPaymentChargeId { get; set; } = default!;

    [JsonPropertyName("provider_payment_charge_id")]
    public string ProviderPaymentChargeId { get; set; } = default!;
}

/// <summary>
/// Bot API PreCheckoutQuery type
/// </summary>
public class BotApiPreCheckoutQuery
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = default!;

    [JsonPropertyName("from")]
    public BotApiUser From { get; set; } = default!;

    [JsonPropertyName("currency")]
    public string Currency { get; set; } = default!;

    [JsonPropertyName("total_amount")]
    public int TotalAmount { get; set; }

    [JsonPropertyName("invoice_payload")]
    public string InvoicePayload { get; set; } = default!;
}

/// <summary>
/// Bot API InlineKeyboardMarkup type
/// </summary>
public class BotApiInlineKeyboardMarkup
{
    [JsonPropertyName("inline_keyboard")]
    public List<List<BotApiInlineKeyboardButton>> InlineKeyboard { get; set; } = default!;
}

/// <summary>
/// Bot API InlineKeyboardButton type
/// </summary>
public class BotApiInlineKeyboardButton
{
    [JsonPropertyName("text")]
    public string Text { get; set; } = default!;

    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyName("callback_data")]
    public string? CallbackData { get; set; }

    [JsonPropertyName("pay")]
    public bool? Pay { get; set; }
}
