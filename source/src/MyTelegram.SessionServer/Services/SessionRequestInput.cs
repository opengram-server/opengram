using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Extended request input that carries session-server-specific fields
/// not present in the base <see cref="IRequestInput"/>.
/// Reconstructed from the original binary's SessionRequestInput.
/// </summary>
public sealed class SessionRequestInput : IRequestInput
{
    public string ConnectionId { get; set; } = string.Empty;
    public Guid RequestId { get; set; }
    public uint ObjectId { get; set; }
    public long UserId { get; set; }
    public long ReqMsgId { get; set; }
    public int SeqNumber { get; set; }
    public long AuthKeyId { get; set; }
    public long PermAuthKeyId { get; set; }
    public int Layer { get; set; }
    public long Date { get; set; }
    public DeviceType DeviceType { get; set; }
    public string ClientIp { get; set; } = string.Empty;
    public long SessionId { get; set; }
    public long AccessHashKeyId { get; set; }

    // Session-server-specific fields
    public ConnectionType ConnectionType { get; set; }
    public long ServerSalt { get; set; }
    public bool IsActive { get; set; }
    public bool Revoked { get; set; }
}
