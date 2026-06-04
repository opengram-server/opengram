namespace MyTelegram.Domain.Shared.Events;

public class ServiceNotificationEvent
{
    public long UserId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool Popup { get; set; }
    public string? MediaUrl { get; set; }
    public string? MediaType { get; set; }
    public bool InvertMedia { get; set; }
    public DateTime Timestamp { get; set; }
}
