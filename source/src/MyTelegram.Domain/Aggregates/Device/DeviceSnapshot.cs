namespace MyTelegram.Domain.Aggregates.Device;

public record DeviceSnapshot(long PermAuthKeyId, long TempAuthKeyId,
    long UserId,
    int ApiId,
    string AppName,
    string AppVersion,
    bool OfficialApp,
    bool PasswordPending,
    string DeviceModel,
    string Platform,
    string SystemVersion,
    string SystemLangCode,
    string LangPack,
    string LangCode,
    string Ip,
    int Layer,
    int Date,
    bool IsActive,
    Dictionary<string, string>? Parameters,
    long Hash
) : ISnapshot;