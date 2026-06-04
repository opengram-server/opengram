using MyTelegram.Schema;

namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Simple DTO event for bot-initiated star gifts (NOT EventFlow domain event)
/// Published to RabbitMQ and handled by Command Server
/// </summary>
public class BotGiftEvent
{
    public long BotUserId { get; set; }
    public long ToUserId { get; set; }
    public long GiftId { get; set; }
    public int Count { get; set; } = 1;
    public string? Message { get; set; }
    public bool HideName { get; set; }
    public bool IncludeUpgrade { get; set; }
    public long Timestamp { get; set; }
    public long RandomId { get; set; }
}
