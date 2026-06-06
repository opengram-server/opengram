namespace MyTelegram.SessionServer.Options;

public class MyTelegramSessionServerOptions
{
    /// <summary>This DC's identifier (e.g. 1, 2, 3…).</summary>
    public int ThisDcId { get; set; } = 1;

    /// <summary>Whether this instance only serves file/media requests.</summary>
    public bool MediaOnly { get; set; }

    /// <summary>gRPC endpoint for the Messenger Command Server.</summary>
    public string CommandServerGrpcUrl { get; set; } = "http://localhost:50051";

    /// <summary>gRPC endpoint for the Messenger Query Server.</summary>
    public string QueryServerGrpcUrl { get; set; } = "http://localhost:50052";

    /// <summary>Object IDs routed to the Command Server.</summary>
    public List<uint> CommandServerObjectIds { get; set; } = new();

    /// <summary>Object IDs routed to the Sticker Server.</summary>
    public List<uint> StickerServerObjectIds { get; set; } = new();

    // --- Rate limiting ---

    /// <summary>Initial token-bucket capacity per session.</summary>
    public int RateLimitCapacity { get; set; } = 100;

    /// <summary>Token refill rate (tokens/second) per session.</summary>
    public int RateLimitTokenRate { get; set; } = 20;

    // --- Temp auth key ---

    /// <summary>How many minutes before a temp auth key expires (default 24 h).</summary>
    public int TempAuthKeyExpirationMinutes { get; set; } = 1440;

    /// <summary>Max channel members allowed for push notifications.</summary>
    public int ChannelMemberMaxCountAllowedForPush { get; set; } = 200;

    /// <summary>Max number of future salts returned by get_future_salts.</summary>
    public int MaxFutureSaltCount { get; set; } = 64;
}
