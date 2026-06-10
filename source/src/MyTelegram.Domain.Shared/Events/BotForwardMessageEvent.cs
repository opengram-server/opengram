namespace MyTelegram.Domain.Shared.Events;

/// <summary>
/// Event for forwarding a message via Bot API.
/// Published to RabbitMQ → handled by Command Server.
/// </summary>
public class BotForwardMessageEvent
{
    public long BotUserId { get; set; }
    public long ToChatId { get; set; }
    public long FromChatId { get; set; }
    public int MessageId { get; set; }
    public bool DisableNotification { get; set; }
    public bool ProtectContent { get; set; }
    public long Timestamp { get; set; }
    public long RandomId { get; set; }
}
