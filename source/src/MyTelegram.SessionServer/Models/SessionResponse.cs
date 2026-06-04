namespace MyTelegram.SessionServer.Models;

public class SessionResponse
{
    public Guid RequestId { get; set; }
    public string Status { get; set; } = "ok"; // "ok" or "error"
    public object? Data { get; set; }
    public string? Error { get; set; }
}
