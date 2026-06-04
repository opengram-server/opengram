namespace MyTelegram.SessionServer.Models;

public class SessionRequest
{
    public Guid RequestId { get; set; }
    public long SessionId { get; set; }
    public long UserId { get; set; }
    public long AuthKeyId { get; set; }
    public string RequestType { get; set; } = string.Empty; // e.g., "create_session", "validate_session"
    public object? Payload { get; set; }
}
