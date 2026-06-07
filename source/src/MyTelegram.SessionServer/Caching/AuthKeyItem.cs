namespace MyTelegram.SessionServer.Caching;

/// <summary>
/// In-memory representation of an authentication key (permanent or temporary).
/// Mirrors the original binary's AuthKeyItem record with 11 properties.
/// </summary>
public sealed class AuthKeyItem
{
    public long PermAuthKeyId { get; set; }
    public long TempAuthKeyId { get; set; }
    public long UserId { get; set; }
    public ReadOnlyMemory<byte> Data { get; set; }
    public long ServerSalt { get; set; }
    public bool IsActive { get; set; }
    public int Layer { get; set; }
    public long AccessHashKeyId { get; set; }
    public DeviceType? DeviceType { get; set; }
    public long ExpiresAt { get; set; }
    public bool IsMediaTempAuthKey { get; set; }

    public AuthKeyItem() { }

    public AuthKeyItem(
        long permAuthKeyId,
        long tempAuthKeyId,
        long userId,
        ReadOnlyMemory<byte> data,
        long serverSalt,
        bool isActive,
        int layer,
        long accessHashKeyId,
        DeviceType? deviceType,
        long expiresAt,
        bool isMediaTempAuthKey)
    {
        PermAuthKeyId = permAuthKeyId;
        TempAuthKeyId = tempAuthKeyId;
        UserId = userId;
        Data = data;
        ServerSalt = serverSalt;
        IsActive = isActive;
        Layer = layer;
        AccessHashKeyId = accessHashKeyId;
        DeviceType = deviceType;
        ExpiresAt = expiresAt;
        IsMediaTempAuthKey = isMediaTempAuthKey;
    }

    /// <summary>Copy constructor.</summary>
    public AuthKeyItem(AuthKeyItem original)
    {
        PermAuthKeyId = original.PermAuthKeyId;
        TempAuthKeyId = original.TempAuthKeyId;
        UserId = original.UserId;
        Data = original.Data;
        ServerSalt = original.ServerSalt;
        IsActive = original.IsActive;
        Layer = original.Layer;
        AccessHashKeyId = original.AccessHashKeyId;
        DeviceType = original.DeviceType;
        ExpiresAt = original.ExpiresAt;
        IsMediaTempAuthKey = original.IsMediaTempAuthKey;
    }
}
