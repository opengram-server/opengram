using MyTelegram.Schema;

namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Simple DTO event for bot messages (NOT EventFlow domain event)
/// Published to RabbitMQ and handled by Command Server
/// </summary>
public class BotMessageEvent
{
    public long BotUserId { get; set; }
    public long ChatId { get; set; }
    public long PeerId { get; set; }
    public PeerType PeerType { get; set; }
    public string Text { get; set; } = "";
    public string? ParseMode { get; set; }
    public bool? DisableWebPagePreview { get; set; }
    public bool? DisableNotification { get; set; }
    public int? ReplyToMessageId { get; set; }
    public string? ReplyMarkupJson { get; set; }  // JSON serialized IReplyMarkup
    public long Timestamp { get; set; }
    public long RandomId { get; set; }
}
