using MyTelegram.Abstractions;

namespace MyTelegram.SessionServer.Services;

/// <summary>
/// Represents an active session keyed by tempAuthKeyId.
/// Reconstructed from the original binary — 15+ fields plus a
/// <see cref="Dictionary{TKey,TValue}"/> of connection items.
/// Held <b>in-memory</b> (not in MongoDB/Redis).
/// </summary>
public sealed class Session
{
    public long TempAuthKeyId { get; set; }
    public long PermAuthKeyId { get; set; }
    public long UserId { get; set; }
    public long AccessHashKeyId { get; set; }
    public long DeviceHash { get; set; }
    public int Layer { get; set; }
    public int CreateDate { get; set; }
    public bool IsOnline { get; set; }
    public bool IsConnectionInitialized { get; set; }
    public bool IsLayerUpdated { get; set; }
    public bool HasGenericSession { get; set; }
    public bool Revoked { get; set; }
    public bool WaitingForLogout { get; set; }
    public PasswordState PasswordState { get; set; }
    public DeviceType DeviceType { get; set; }
    public TokenBucket? TokenBucket { get; set; }
    public long PushSessionId { get; set; }

    // InitConnection-provided app info
    public int ApiId { get; set; }
    public string AppName { get; set; } = string.Empty;
    public string DeviceModel { get; set; } = string.Empty;
    public string SystemVersion { get; set; } = string.Empty;
    public string LangCode { get; set; } = string.Empty;
    public string LangPack { get; set; } = string.Empty;
    public string SystemLangCode { get; set; } = string.Empty;

    /// <summary>Connections keyed by connectionId.</summary>
    public Dictionary<string, SessionItem> Items { get; } = new();
}

/// <summary>
/// A single TCP/WebSocket connection within a <see cref="Session"/>.
/// </summary>
public sealed class SessionItem
{
    public string ConnectionId { get; set; } = string.Empty;
    public long SessionId { get; set; }
    public long ServerSalt { get; set; }
    public ConnectionType ConnectionType { get; set; }

    public SessionItem() { }

    public SessionItem(SessionItem original)
    {
        ConnectionId = original.ConnectionId;
        SessionId = original.SessionId;
        ServerSalt = original.ServerSalt;
        ConnectionType = original.ConnectionType;
    }
}
